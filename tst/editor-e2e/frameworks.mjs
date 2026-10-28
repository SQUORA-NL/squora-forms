import { chromium } from 'playwright';
import fs from 'fs';
// The editor's own behaviour (drop targets, selection, the properties pane, click to add) must be the same whichever UI library draws the form,
// so every test here runs once per framework. battle.mjs goes deeper into the property editor, on MudBlazor.
const OUT_DIR = process.env.OUT_DIR || './out/';
const APP_LOG = process.env.APP_LOG || './app.log';
const URL = process.env.APP_URL || 'http://localhost:5199/';
const FRAMEWORKS = (process.env.FRAMEWORKS || 'MudBlazor,HTML,Radzen').split(',');
fs.mkdirSync(OUT_DIR, { recursive: true });
const browser = await chromium.launch();
let failed = 0, total = 0;
let errors = [];
const logText = () => fs.readFileSync(APP_LOG, 'utf8');
const circuitFails = t => (t.match(/Unhandled exception rendering component: [^\n]*/g) || []);
const settle = p => p.waitForTimeout(450);

async function fresh(framework, template = 'Address', viewport = { width: 1500, height: 1100 }) {
  const ctx = await browser.newContext({ viewport });
  const p = await ctx.newPage();
  errors = [];
  p.on('console', m => { if (m.type() === 'error' && !/404/.test(m.text())) errors.push('console: ' + m.text().slice(0, 200)); });
  p.on('pageerror', e => errors.push('pageerror: ' + e.message.slice(0, 200)));
  await p.goto(URL, { waitUntil: 'networkidle' });
  await p.waitForTimeout(700);
  await p.locator('.demo-framework').getByText(framework, { exact: true }).click();
  await p.waitForTimeout(800);
  await p.getByRole('button', { name: 'Templates' }).click();
  await p.getByText(template, { exact: true }).click();
  await p.waitForTimeout(900);
  return { p, ctx };
}

async function test(framework, name, fn, opts = {}) {
  const full = `${framework}: ${name}`;
  if (process.env.ONLY && !new RegExp(process.env.ONLY).test(full)) return;
  total++;
  const logBefore = circuitFails(logText()).length;
  const { p, ctx } = await fresh(framework, opts.template, opts.viewport);
  let status = 'PASS', note = '';
  try { const r = await fn(p); if (r) { status = 'FAIL'; note = r; } }
  catch (e) { status = 'ERROR'; note = e.message.split('\n')[0].slice(0, 250); }
  await new Promise(r => setTimeout(r, 300));
  const newFails = circuitFails(logText()).slice(logBefore);
  if (newFails.length) { status = 'FAIL'; note += ' [SERVER: ' + newFails[0].slice(0, 190) + ']'; }
  const errUi = await p.evaluate(() => { const e = document.getElementById('blazor-error-ui'); return e && getComputedStyle(e).display !== 'none'; }).catch(() => false);
  if (errUi) { status = 'FAIL'; note += ' [blazor-error-ui shown]'; }
  if (errors.length && status === 'PASS') { status = 'WARN'; note += ' [' + errors.join(' | ') + ']'; }
  if (status !== 'PASS') { failed++; await p.screenshot({ path: OUT_DIR + 'fw-' + full.replace(/\W+/g, '_') + '.png', fullPage: true }).catch(() => {}); }
  console.log(status.padEnd(5), full, note ? '— ' + note : '');
  await ctx.close();
}

// ---- helpers (nothing here depends on the UI library) ----
const fields = p => p.locator('.bd-editable:not(.bd-container)');
const layouts = p => p.locator('.bd-editable.bd-container');
const paneTitle = p => p.locator('.bd-property-editor-header:not(.bd-pe-plain) h4').innerText();
const hasSelection = p => p.locator('.bd-selected').count();
async function pick(p, loc, pos = { x: 6, y: 6 }) { await loc.click({ position: pos }); await settle(p); }
async function mouseDrag(p, from, to, { release = true, steps = 14 } = {}) {
  await p.mouse.move(from.x, from.y); await p.mouse.down();
  await p.mouse.move(from.x + 30, from.y + 30, { steps: 4 });
  await p.mouse.move(to.x, to.y, { steps }); await p.waitForTimeout(500);
  if (release) { await p.mouse.up(); await p.waitForTimeout(700); }
}
async function toolboxItem(p, name) { const bb = await p.locator('.bd-toolbox-item', { hasText: new RegExp('^\\s*' + name + '\\s*$') }).first().boundingBox(); return { x: bb.x + bb.width / 2, y: bb.y + bb.height / 2 }; }
async function lower(p, loc) { const bb = await loc.boundingBox(); return { x: bb.x + bb.width / 2, y: bb.y + bb.height * 0.8 }; }
async function monacoValue(p) {
  await p.waitForFunction(() => window.monaco && monaco.editor.getEditors().length, null, { timeout: 15000 });
  return await p.evaluate(() => monaco.editor.getEditors().map(e => e.getValue()).join('\n'));
}

for (const fw of FRAMEWORKS) {
  await test(fw, 'the template renders in edit mode with the editor around it', async p => {
    if ((await fields(p).count()) !== 5) return 'fields: ' + (await fields(p).count());
    if (!(await p.locator('.bd-toolbox-item').count())) return 'no toolbox';
    if (!(await p.locator('.bd-property-sidebar').count())) return 'no properties pane';
    if (!(await p.locator('.bd-editor-hint').innerText()).includes('Click an element')) return 'no how-to hint in the bar';
  });

  await test(fw, 'idle: no drop targets drawn, no tag shown', async p => {
    if (await p.locator('.bd-zone').count()) return 'zones while idle';
    const shown = await p.locator('.bd-tag').evaluateAll(ts => ts.filter(t => getComputedStyle(t).opacity === '1').length);
    if (shown) return shown + ' tags visible while idle';
  });

  await test(fw, 'hover: the innermost element gets a tag with its kind', async p => {
    await fields(p).nth(2).hover(); await settle(p);
    const tags = await p.locator('.bd-tag').evaluateAll(ts => ts.filter(t => getComputedStyle(t).opacity === '1').map(t => t.innerText.replace(/\s+/g, ' ').trim()));
    if (tags.length !== 1) return 'visible tags: ' + JSON.stringify(tags);
    if (!/String|Text Field/.test(tags[0])) return 'tag says: ' + tags[0];
  });

  await test(fw, 'click: selects, opens the pane on that element, shows the action bar', async p => {
    await pick(p, fields(p).nth(0));
    if ((await hasSelection(p)) !== 1) return 'selected count ' + (await hasSelection(p));
    if (!/Street/.test(await paneTitle(p))) return 'pane title: ' + (await paneTitle(p));
    const act = await p.locator('.bd-selected > .bd-tag .bd-act').count();
    if (act !== 4) return 'action buttons: ' + act;
  });

  await test(fw, 'empty pane tells what to do; selecting fills it', async p => {
    const t = await p.locator('.bd-property-editor-empty').innerText();
    if (!/Nothing selected/.test(t)) return 'empty state: ' + t.slice(0, 60);
    await pick(p, fields(p).nth(1));
    if (await p.locator('.bd-property-editor-empty').count()) return 'empty state stays';
  });

  await test(fw, 'Properties button hides and shows the pane; a click on an element reopens it', async p => {
    await p.locator('.bd-pane-toggle').click(); await settle(p);
    if (await p.locator('.bd-property-sidebar').count()) return 'not hidden';
    if ((await p.locator('.bd-pane-toggle').getAttribute('aria-pressed')) !== 'false') return 'button state does not follow';
    await pick(p, fields(p).nth(1));
    if (!(await p.locator('.bd-property-sidebar').count())) return 'selecting did not reopen the pane';
    if ((await p.locator('.bd-pane-toggle').getAttribute('aria-pressed')) !== 'true') return 'button state does not follow (open)';
  });

  await test(fw, 'action bar: the sliders button opens the pane at the label', async p => {
    await pick(p, fields(p).nth(1));
    await p.locator('.bd-pane-toggle').click(); await settle(p); // hide
    await p.locator('.bd-selected > .bd-tag .bd-act-props').click(); await settle(p);
    if (!(await p.locator('.bd-property-sidebar').count())) return 'pane not opened';
    const focused = await p.evaluate(() => document.activeElement?.className || '');
    if (!/bd-property-input/.test(focused)) return 'focus is on: ' + focused;
  });

  await test(fw, 'double click selects and puts the cursor in the label', async p => {
    await fields(p).nth(2).dblclick({ position: { x: 6, y: 6 } }); await settle(p);
    const focused = await p.evaluate(() => document.activeElement?.className || '');
    if (!/bd-property-input/.test(focused)) return 'focus is on: ' + focused;
  });

  await test(fw, 'action bar: select parent', async p => {
    await pick(p, fields(p).nth(0));
    await p.locator('.bd-selected > .bd-tag .bd-act-parent').click(); await settle(p);
    if (!/Horizontal Layout|Vertical Layout/.test(await p.locator('.bd-pe-subtitle').innerText())) return 'parent not selected: ' + await p.locator('.bd-pe-subtitle').innerText();
  });

  await test(fw, 'action bar: duplicate gives a selected copy with its own path', async p => {
    const before = await fields(p).count();
    await pick(p, fields(p).nth(0));
    await p.locator('.bd-selected > .bd-tag .bd-act-copy').click(); await settle(p);
    if ((await fields(p).count()) !== before + 1) return 'count ' + (await fields(p).count());
    await p.locator('.bd-json-toggle-button').click(); await settle(p);
    const json = JSON.parse(await p.locator('.bd-json-editor').inputValue());
    if (json.Parameters.Path !== '$.address.street2') return 'copy path: ' + json.Parameters.Path;
  });

  await test(fw, 'action bar: delete removes it, the pane goes back to empty', async p => {
    const before = await fields(p).count();
    await pick(p, fields(p).nth(1));
    await p.locator('.bd-selected > .bd-tag .bd-act-delete').click(); await settle(p);
    if ((await fields(p).count()) !== before - 1) return 'count ' + (await fields(p).count());
    if (!(await p.locator('.bd-property-editor-empty').count())) return 'pane not empty';
  });

  await test(fw, 'Escape clears the selection', async p => {
    await pick(p, fields(p).nth(1));
    await p.keyboard.press('Escape'); await settle(p);
    if (await hasSelection(p)) return 'still selected';
  });

  await test(fw, 'click adds after the selected field and selects the new one', async p => {
    await pick(p, fields(p).nth(1));
    const hint = await p.locator('.bd-toolbox-target').innerText();
    if (!/after Number/.test(hint)) return 'toolbox says: ' + hint;
    await p.locator('.bd-toolbox-item', { hasText: /^\s*Checkbox\s*$/ }).click(); await settle(p);
    if ((await fields(p).count()) !== 6) return 'count ' + (await fields(p).count());
    if (!/New Checkbox/.test(await paneTitle(p))) return 'selected: ' + (await paneTitle(p));
  });

  await test(fw, 'click adds into the selected layout; nothing selected adds at the end of the form', async p => {
    await pick(p, fields(p).nth(2)); // postal code, in the second row
    await p.locator('.bd-selected > .bd-tag .bd-act-parent').click(); await settle(p); // select the row
    if (!/inside/.test(await p.locator('.bd-toolbox-target').innerText())) return 'toolbox says: ' + (await p.locator('.bd-toolbox-target').innerText());
    await p.locator('.bd-toolbox-item', { hasText: /^\s*Text Field\s*$/ }).click(); await settle(p);
    const n = await layouts(p).nth(2).locator('.bd-editable:not(.bd-container)').count();
    if (n !== 3) return 'row now has ' + n;
    await p.keyboard.press('Escape'); await settle(p);
    if (!/end of the form/.test(await p.locator('.bd-toolbox-target').innerText())) return 'toolbox says: ' + (await p.locator('.bd-toolbox-target').innerText());
  });

  await test(fw, 'toolbox drag: every layout and spot is shown, the one under the pointer is named', async p => {
    const target = fields(p).nth(4); // the dropdown
    await mouseDrag(p, await toolboxItem(p, 'Checkbox'), await lower(p, target), { release: false });
    const zones = await p.locator('.bd-zone').count();
    const outlined = await p.locator('.bd-drag-active.bd-container').count();
    const hint = await p.locator('.bd-zone.over').first().getAttribute('data-hint');
    const into = await p.locator('.bd-drop-target').first().getAttribute('data-drop-label');
    await p.mouse.up(); await p.waitForTimeout(700);
    if (zones < 8) return 'only ' + zones + ' drop spots shown';
    if (outlined < 3) return 'layouts outlined: ' + outlined;
    if (!/^Drop (above|below|left of|right of) /.test(hint || '')) return 'hint: ' + hint;
    if (!/^Into /.test(into || '')) return 'target label: ' + into;
    if ((await fields(p).count()) !== 6) return 'drop did not add: ' + (await fields(p).count());
  });

  await test(fw, 'after the drop: nothing drawn is left behind', async p => {
    await mouseDrag(p, await toolboxItem(p, 'Integer'), await lower(p, fields(p).nth(1)));
    if (await p.locator('.bd-zone, .bd-drop-target, .bd-drag-active').count()) return 'leftover drag state';
  });

  await test(fw, 'drag released on nothing changes nothing and clears the drag state', async p => {
    await mouseDrag(p, await toolboxItem(p, 'Checkbox'), { x: 700, y: 12 });
    if ((await fields(p).count()) !== 5) return 'count ' + (await fields(p).count());
    if (await p.locator('.bd-zone, .bd-drop-target, .bd-drag-active').count()) return 'leftover drag state';
  });

  await test(fw, 'move by the handle: the element lands where the pill said', async p => {
    await fields(p).nth(4).hover(); await settle(p); // dropdown
    const h = await p.locator('.bd-tag').evaluateAll(ts => { const t = ts.find(t => getComputedStyle(t).opacity === '1'); const r = t.querySelector('.bd-handle').getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2 }; });
    const first = fields(p).nth(0);
    const bb = await first.boundingBox();
    await mouseDrag(p, h, { x: bb.x + bb.width * 0.25, y: bb.y + 3 }, { release: false });
    const hint = await p.locator('.bd-zone.over').first().getAttribute('data-hint');
    await p.mouse.up(); await p.waitForTimeout(700);
    if (!/Street/.test(hint || '')) return 'hint: ' + hint;
    const inFirstRow = await layouts(p).nth(1).locator('.bd-editable:not(.bd-container)').count();
    if (inFirstRow !== 3) return 'the first row has ' + inFirstRow + ' fields';
  });

  // a move redraws the list, so the handle that started it can be gone before its dragend arrives: the drop has to end the drag itself
  await test(fw, 'after moving by the handle: nothing drawn is left behind, and it can be moved again', async p => {
    const handleOf = async f => { await f.hover(); await settle(p); return await p.locator('.bd-tag').evaluateAll(ts => { const t = ts.find(t => getComputedStyle(t).opacity === '1'); const r = t.querySelector('.bd-handle').getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2 }; }); };
    const leftover = () => p.locator('.bd-zone, .bd-drop-target, .bd-drag-active, .bd-dragging, .bd-shield').count();
    const bb = await fields(p).nth(0).boundingBox();
    await mouseDrag(p, await handleOf(fields(p).nth(4)), { x: bb.x + bb.width * 0.25, y: bb.y + 3 }); // the dropdown to the first row
    if (await leftover()) return 'drag state left behind after the first move: ' + (await leftover());
    await mouseDrag(p, await handleOf(fields(p).nth(0)), await lower(p, fields(p).nth(4))); // and the same one again, to the end
    if (await leftover()) return 'drag state left behind after the second move: ' + (await leftover());
    if ((await fields(p).count()) !== 5) return 'fields: ' + (await fields(p).count());
  });

  await test(fw, 'dropping into an empty layout: placeholder is shown, lights up and takes the drop', async p => {
    await p.locator('.bd-toolbox-item', { hasText: /^\s*Vertical Layout\s*$/ }).click(); await settle(p);
    const ph = p.locator('.bd-empty');
    if ((await ph.count()) !== 1) return 'placeholders: ' + (await ph.count());
    await mouseDrag(p, await toolboxItem(p, 'Text Field'), await (async () => { const b = await ph.boundingBox(); return { x: b.x + b.width / 2, y: b.y + b.height / 2 }; })(), { release: false });
    const over = await p.locator('.bd-empty.over').count();
    await p.mouse.up(); await p.waitForTimeout(700);
    if (!over) return 'placeholder did not light up';
    if (await p.locator('.bd-empty').count()) return 'placeholder still there after the drop';
    if ((await fields(p).count()) !== 6) return 'count ' + (await fields(p).count());
  });

  await test(fw, 'a layout cannot be dropped into itself', async p => {
    const row = layouts(p).nth(1); // the first row (the group around it is the form itself, which has no handle)
    await row.hover({ position: { x: 2, y: 2 } }); await settle(p);
    const h = await p.locator('.bd-tag').evaluateAll(ts => { const t = ts.find(t => getComputedStyle(t).opacity === '1'); const r = t.querySelector('.bd-handle').getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2 }; });
    const inner = await fields(p).nth(0).boundingBox();
    await mouseDrag(p, h, { x: inner.x + inner.width / 2, y: inner.y + inner.height - 3 }, { release: false });
    const offered = await p.locator('.bd-zone.over').count();
    await p.mouse.up(); await p.waitForTimeout(700);
    if (offered) return 'a drop spot inside the dragged row was offered';
    if ((await fields(p).count()) !== 5) return 'count ' + (await fields(p).count());
    if ((await layouts(p).count()) !== 3) return 'layouts: ' + (await layouts(p).count());
    if ((await layouts(p).nth(1).locator('.bd-editable:not(.bd-container)').count()) !== 2) return 'the row changed';
  });

  await test(fw, 'the framework can be switched while a field is selected, data and form stay', async p => {
    await fields(p).nth(0).locator('input').first().fill('Main street'); await p.keyboard.press('Tab'); await settle(p);
    const other = FRAMEWORKS.find(f => f !== fw);
    await p.locator('.demo-framework').getByText(other, { exact: true }).click(); await p.waitForTimeout(1200);
    if ((await fields(p).count()) !== 5) return 'fields after switch: ' + (await fields(p).count());
    const v = await monacoValue(p);
    if (!v.includes('Main street')) return 'data lost: ' + v.slice(0, 100);
    await fields(p).nth(1).locator('input').first().fill('7').catch(() => {});
    await p.locator('.demo-framework').getByText(fw, { exact: true }).click(); await p.waitForTimeout(1200);
    if ((await fields(p).count()) !== 5) return 'fields after switching back: ' + (await fields(p).count());
  });

  await test(fw, 'accent colour reaches the editor', async p => {
    await p.locator('.demo-swatch[title="Rose"]').click(); await settle(p);
    await pick(p, fields(p).nth(0));
    const c = await p.locator('.bd-selected').first().evaluate(e => getComputedStyle(e).outlineColor);
    if (c !== 'rgb(225, 29, 72)') return 'selection outline is ' + c;
  });

  await test(fw, 'view mode: no chrome, form still there; back to edit restores it', async p => {
    await p.locator('.bd-toggle-slider').click(); await settle(p);
    if (await p.locator('.bd-toolbox-item, .bd-property-sidebar, .bd-tag, .bd-zone').count()) return 'chrome in view mode';
    if (!(await p.locator('.sq-dynamicform input, .sq-dynamicform select').count())) return 'form gone';
    await p.locator('.bd-toggle-slider').click(); await settle(p);
    if (!(await p.locator('.bd-toolbox-item').count())) return 'editor not back';
  });

  await test(fw, 'narrow screen: components above the form, pane as a sheet, no sideways scroll', async p => {
    const w = await p.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
    const rail = await p.locator('.bd-toolbox-rail').boundingBox();
    const canvas = await p.locator('.bd-canvas').boundingBox();
    if (!(rail.y + rail.height <= canvas.y + 2)) return 'toolbox is not above the form';
    const pane = await p.locator('.bd-property-sidebar').evaluate(e => getComputedStyle(e).position);
    if (pane !== 'fixed') return 'pane is ' + pane;
    if (w > 1) return 'sideways scroll of ' + w + 'px';
  }, { viewport: { width: 700, height: 900 } });
}

console.log(`\n${total - failed}/${total} passed`);
await browser.close();
process.exit(failed ? 1 : 0);
