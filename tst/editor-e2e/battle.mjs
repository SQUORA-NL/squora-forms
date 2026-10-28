import { chromium } from 'playwright';
import fs from 'fs';
// APP_LOG is the console output of the sample app (a circuit that dies is only visible there); failure screenshots go to OUT_DIR
const OUT_DIR = process.env.OUT_DIR || './out/';
const APP_LOG = process.env.APP_LOG || './app.log';
const URL = process.env.APP_URL || 'http://localhost:5199/';
fs.mkdirSync(OUT_DIR, { recursive: true });
const browser = await chromium.launch();
const results = [];
let errors = [];
const logText = () => fs.readFileSync(APP_LOG, 'utf8');
const circuitFails = t => (t.match(/Unhandled exception rendering component: [^\n]*/g) || []);

async function fresh() {
  const ctx = await browser.newContext({ viewport: { width: 1500, height: 1300 } });
  const p = await ctx.newPage();
  errors = [];
  p.on('console', m => { if (m.type() === 'error' && !/404/.test(m.text())) errors.push('console: ' + m.text().slice(0, 200)); });
  p.on('pageerror', e => errors.push('pageerror: ' + e.message.slice(0, 200)));
  await p.goto(URL, { waitUntil: 'networkidle' });
  await p.waitForTimeout(800);
  // the page starts with an empty form: load a template to work on
  await p.getByRole('button', { name: 'Templates' }).click();
  await p.getByText(process.env.TEMPLATE || 'Profile', { exact: true }).click();
  await p.waitForTimeout(800);
  // the group is open in edit mode already; click Expand only if the page starts collapsed
  const expand = p.getByRole('button', { name: 'Expand' });
  if (await expand.count()) await expand.click();
  await p.waitForTimeout(500);
  return { p, ctx };
}
async function errorUi(p) {
  return await p.evaluate(() => { const e = document.getElementById('blazor-error-ui'); return e && getComputedStyle(e).display !== 'none'; });
}
const settle = p => p.waitForTimeout(500);
// The default template's root is a (collapsible) group, which is an editable element of its own and selectable like any other.
// The tests count the elements inside it, so index 0 is the layout in it and 1 the first field: this leaves the root group out.
const hasClass = c => `contains(concat(' ',normalize-space(@class),' '),' ${c} ')`;
const editable = p => p.locator(`xpath=//*[${hasClass('bd-editable')}][not(${hasClass('groupcomponent')} and not(ancestor::*[${hasClass('bd-editable')}]))]`);

const refresh = async p => { await p.locator('.bd-toggle-slider').click(); await p.waitForTimeout(500); await p.locator('.bd-toggle-slider').click(); await p.waitForTimeout(600); const ex = p.getByRole('button', { name: 'Expand' }); if (await ex.count()) { await ex.click(); await p.waitForTimeout(400); } };
// index 0 is the vertical layout, which has no area of its own anymore (edit mode adds no padding): click in the gap under its first child
async function select(p, i) {
  if (i === 0) {
    const k = p.locator('.verticalcomponent').first().locator(':scope > [role=group] > .bd-editable');
    const a = await k.nth(0).boundingBox(), b = await k.nth(1).boundingBox();
    await p.mouse.click(a.x + a.width / 2, (a.y + a.height + b.y) / 2);
  } else {
    await editable(p).nth(i).click({ position: { x: 5, y: 5 } });
  }
  await settle(p);
}
async function elementJson(p) {
  if (!(await p.locator('.bd-json-editor').count())) { await p.locator('.bd-json-toggle-button').click(); await settle(p); }
  return await p.locator('.bd-json-editor').inputValue();
}
async function applyJson(p, text) {
  await p.locator('.bd-json-editor').fill(text);
  await p.locator('.bd-apply-json-button').click();
  await settle(p);
}
async function fullFormMode(p) {
  if (!(await p.locator('.bd-json-editor').count())) { await p.locator('.bd-json-toggle-button').click(); await settle(p); }
  const t = (await p.locator('.bd-json-mode-button').innerText()).trim();
  if (t === 'Element') { await p.locator('.bd-json-mode-button').click(); await settle(p); } // button label says what you switch *to*? check below
}

async function test(name, fn) {
  if (process.env.ONLY && !new RegExp(process.env.ONLY).test(name)) return;
  const logBefore = circuitFails(logText()).length;
  const { p, ctx } = await fresh();
  let status = 'PASS', note = '';
  try { const r = await fn(p); if (r) { status = 'FAIL'; note = r; } }
  catch (e) { status = 'ERROR'; note = e.message.split('\n')[0].slice(0, 250); }
  await new Promise(r => setTimeout(r, 300));
  const newFails = circuitFails(logText()).slice(logBefore);
  if (newFails.length) { status = status === 'ERROR' ? 'ERROR' : 'FAIL'; note += ' [SERVER: ' + newFails[0].slice(0, 190) + ']'; }
  if (await errorUi(p)) { status = status === 'PASS' ? 'FAIL' : status; note += ' [blazor-error-ui shown]'; }
  if (errors.length) { note += ' [' + errors.join(' | ') + ']'; if (status === 'PASS') status = 'WARN'; }
  if (status !== 'PASS') await p.screenshot({ path: OUT_DIR + 'fail-' + name.replace(/\W+/g, '_') + '.png', fullPage: true }).catch(() => {});
  results.push({ name, status, note });
  console.log(status.padEnd(5), name, note ? '— ' + note : '');
  await ctx.close();
}
const labels = p => p.locator('.bd-form-container-fullwidth label.mud-input-label, .bd-form-container-fullwidth p.mud-typography').allTextContents().then(a => a.map(x => x.trim()));
const kids = p => p.locator('.verticalcomponent').first().locator(':scope > [role=group] > .bd-editable');
const rootChildren = p => kids(p).count();

// ---------- selection ----------
await test('select string field shows Text Field props', async p => {
  await select(p, 1);
  const t = await p.locator('.bd-property-editor').innerText();
  if (!/Text Field/.test(t) || !(await p.locator('input.bd-property-input').count())) return 'props missing: ' + t.slice(0, 100);
});
await test('select layout shows Vertical Layout, no label/path', async p => {
  await select(p, 0);
  const t = await p.locator('.bd-property-editor').innerText();
  if (!/Vertical Layout/.test(t)) return 'wrong type: ' + t.slice(0, 120);
  // every element has a CSS class field (and one in a row a width): a layout just has no label or path of its own
  const asked = (await p.locator('.bd-property-label').allInnerTexts()).map(x => x.trim());
  if (asked.some(x => x === 'Label' || x === 'Path')) return 'layout shows label/path inputs: ' + asked.join(', ');
});
await test('nested click selects only innermost', async p => {
  await editable(p).nth(1).locator('input').first().click();
  await settle(p);
  const t = await p.locator('.bd-property-editor').innerText();
  if (!/Text Field/.test(t)) return 'inner click selected: ' + t.slice(0, 80);
});
await test('select FileUpload shows props', async p => {
  await select(p, 3);
  const t = await p.locator('.bd-property-editor').innerText();
  if (/Unknown/.test(t)) return 'FileUpload shows "Unknown" type name';
  if (!/File Upload/.test(t)) return 'no type name in the pane: ' + t.slice(0, 80);
});
await test('background click clears selection', async p => {
  await select(p, 1);
  const bb = await p.locator('.bd-canvas').boundingBox();
  // the canvas around the form's page
  await p.mouse.click(bb.x + 8, bb.y + 8);
  await settle(p);
  if (await p.locator('.bd-property-editor-header:not(.bd-pe-plain)').count()) return 'selection not cleared by clicking the canvas background';
});
await test('click on empty form-area space clears selection', async p => {
  await select(p, 1);
  const bb = await p.locator('.bd-form-container-fullwidth').boundingBox();
  await p.mouse.click(bb.x + bb.width - 10, bb.y + bb.height - 5);
  await settle(p);
  if (await p.locator('.bd-property-editor-header:not(.bd-pe-plain)').count()) return 'selection not cleared';
});
await test('Escape clears the selection', async p => {
  await select(p, 1);
  await p.keyboard.press('Escape'); await settle(p);
  if (await p.locator('.bd-property-editor-header:not(.bd-pe-plain)').count()) return 'still selected';
});
await test('pane close button hides the pane, the selection stays', async p => {
  await select(p, 1);
  await p.locator('.bd-close-button').click(); await settle(p);
  if (await p.locator('.bd-property-sidebar').count()) return 'pane still open';
  if (!(await p.locator('.bd-selected').count())) return 'selection lost';
  await p.locator('.bd-pane-toggle').click(); await settle(p);
  if (!(await p.locator('.bd-property-editor-header:not(.bd-pe-plain)').count())) return 'pane did not come back with the selection';
});
await test('selected element has visual highlight', async p => {
  await select(p, 1);
  const cls = await editable(p).nth(1).getAttribute('class');
  const any = await p.locator('[class*="selected"]').count();
  if (!/selected/.test(cls || '') && !any) return 'no selected class anywhere: ' + cls;
});

// ---------- property edits ----------
await test('edit Label updates form', async p => {
  await select(p, 1);
  await p.locator('input.bd-property-input').first().fill('Voornaam');
  await p.keyboard.press('Tab'); await settle(p);
  const l = await labels(p);
  if (!l.includes('Voornaam')) return 'label not rendered: ' + l.join(',');
});
await test('edit Path reflected in JSON', async p => {
  await select(p, 1);
  await p.locator('input.bd-property-input').nth(1).fill('$.person.first');
  await p.keyboard.press('Tab'); await settle(p);
  const j = await elementJson(p);
  if (!j.includes('$.person.first')) return j;
});
await test('edited Path binds data (typing → data panel)', async p => {
  await select(p, 1);
  await p.locator('input.bd-property-input').nth(1).fill('$.zzz');
  await p.keyboard.press('Tab'); await settle(p);
  await editable(p).nth(1).locator('input').first().fill('hello');
  await p.keyboard.press('Tab'); await settle(p);
  const body = (await p.waitForFunction(() => window.monaco && monaco.editor.getEditors().length, null, { timeout: 15000 }), await p.evaluate(() => monaco.editor.getEditors().map(e => e.getValue()).join('\n')));
  if (!body.includes('"zzz"')) return 'data panel: ' + body.slice(0, 120);
});
await test('empty label does not crash', async p => {
  await select(p, 1);
  await p.locator('input.bd-property-input').first().fill('');
  await p.keyboard.press('Tab'); await settle(p);
});
await test('special chars / HTML in label are escaped', async p => {
  await select(p, 1);
  await p.locator('input.bd-property-input').first().fill('<img src=x onerror=window.__xss=1>"\'&');
  await p.keyboard.press('Tab'); await settle(p);
  if (await p.evaluate(() => window.__xss)) return 'XSS executed';
  if (await p.locator('.bd-form-container-fullwidth img[src="x"]').count()) return 'raw img rendered';
});
await test('very long label (5000 chars)', async p => {
  await select(p, 1);
  await p.locator('input.bd-property-input').first().fill('x'.repeat(5000));
  await p.keyboard.press('Tab'); await settle(p);
});
await test('unicode/emoji label', async p => {
  await select(p, 1);
  await p.locator('input.bd-property-input').first().fill('Naam 名前 🚀 é');
  await p.keyboard.press('Tab'); await settle(p);
  if (!(await labels(p)).includes('Naam 名前 🚀 é')) return 'not rendered';
});
await test('property panel follows selection (no stale values)', async p => {
  await select(p, 1);
  const a = await p.locator('input.bd-property-input').first().inputValue();
  await select(p, 2);
  const b = await p.locator('input.bd-property-input').first().inputValue();
  if (a === b) return `same label ${a} for two fields`;
});

// ---------- JSON ----------
await test('element JSON: apply valid edit', async p => {
  await select(p, 1);
  const j = JSON.parse(await elementJson(p));
  j.Parameters.Label = 'ViaJson';
  await applyJson(p, JSON.stringify(j));
  if (!(await labels(p)).includes('ViaJson')) return 'not rendered';
});
await test('element JSON: invalid JSON shows error, model intact', async p => {
  await select(p, 1);
  await elementJson(p);
  await applyJson(p, '{ not json');
  if (!(await p.locator('.bd-json-error').count())) return 'no error shown';
  if ((await labels(p)).includes('{')) return 'model corrupted';
});
await test('element JSON: empty text', async p => {
  await select(p, 1); await elementJson(p);
  await applyJson(p, '');
});
await test('element JSON: valid JSON but not a model ("[]", "null", "42")', async p => {
  await select(p, 1); await elementJson(p);
  for (const t of ['[]', 'null', '42', '{}']) { await applyJson(p, t); }
  const body = await p.locator('.bd-form-container-fullwidth').innerText();
  return '';
});
await test('element JSON: unknown ComponentType', async p => {
  await select(p, 1); await elementJson(p);
  await applyJson(p, '{"DynamicType":{"ComponentType":"Bogus"},"Parameters":{}}');
  if (!(await p.locator('.bd-json-error').count())) return 'accepted unknown type silently (no error)';
});
await test('element JSON: change type String→Boolean', async p => {
  await select(p, 1);
  const j = JSON.parse(await elementJson(p));
  j.DynamicType.ComponentType = 'Boolean';
  await applyJson(p, JSON.stringify(j));
  if (!(await p.locator('.bd-form-container-fullwidth input[type=checkbox], .mud-checkbox').count())) return 'no checkbox rendered';
});
await test('full-form JSON roundtrip: apply unchanged', async p => {
  await select(p, 1); await elementJson(p);
  await p.locator('.bd-json-mode-button').click(); await settle(p);
  const before = await p.locator('.bd-json-editor').inputValue();
  if (!before.includes('Collapsable')) return 'not full form: ' + before.slice(0, 60);
  const n = await rootChildren(p);
  await p.locator('.bd-apply-json-button').click(); await settle(p);
  if ((await rootChildren(p)) !== n) return 'child count changed after no-op apply';
});
await test('full-form JSON: unchanged apply, then re-render (root with HeaderModel)', async p => {
  await select(p, 1); await elementJson(p);
  await p.locator('.bd-json-mode-button').click(); await settle(p);
  await p.locator('.bd-apply-json-button').click(); await settle(p);
  await p.locator('.bd-toggle-slider').click(); await settle(p);
  if ((await p.locator('.bd-toggle-text').innerText().catch(() => 'gone')) !== 'Test Mode') return 'edit-mode toggle dead after apply (circuit crashed)';
});
await test('full-form JSON: apply twice does not duplicate elements', async p => {
  await select(p, 1); await elementJson(p);
  await p.locator('.bd-json-mode-button').click(); await settle(p);
  const n = await rootChildren(p);
  for (let i = 0; i < 2; i++) {
    const t = await p.locator('.bd-json-editor').inputValue().catch(() => null);
    if (t === null) { await select(p, 1); await elementJson(p); await p.locator('.bd-json-mode-button').click(); await settle(p); }
    await p.locator('.bd-apply-json-button').click(); await settle(p);
  }
  if ((await rootChildren(p)) !== n) return `children ${n} → ${await rootChildren(p)}`;
});
await test('full-form JSON: rich form with every component type renders', async p => {
  await select(p, 1); await elementJson(p);
  await p.locator('.bd-json-mode-button').click(); await settle(p);
  const m = JSON.parse(await p.locator('.bd-json-editor').inputValue()); delete m.Parameters.HeaderModel; m.DynamicType.TypeDefinitionName = '';
  const mk = (t, label, path, extra = {}) => ({ DynamicType: { ComponentType: t, TypeDefinitionName: '' }, Parameters: { Label: label, Path: path, ...extra } });
  m.Elements = [{ DynamicType: { ComponentType: 'VerticalLayout', TypeDefinitionName: '' }, Elements: [
    mk('String', 'S', '$.s'), mk('Boolean', 'B', '$.b'), mk('Int', 'I', '$.i'), mk('Number', 'N', '$.n'),
    mk('DateTime', 'D', '$.d'), mk('Dropdown', 'DD', '$.dd', { Options: { a: 'Alpha', b: 'Beta' } }),
    { DynamicType: { ComponentType: 'HorizontalLayout', TypeDefinitionName: '' }, Elements: [mk('String', 'H1', '$.h1'), mk('String', 'H2', '$.h2')] },
    { DynamicType: { ComponentType: 'GroupLayout', TypeDefinitionName: '' }, Parameters: { Label: 'Grp' }, Elements: [mk('String', 'G1', '$.g1')] },
  ] }];
  await applyJson(p, JSON.stringify(m)); await refresh(p);
  if (await p.locator('.bd-json-error').count()) return 'error shown';
  const l = (await labels(p)).join('|');
  for (const x of ['S', 'B', 'I', 'N', 'D', 'DD', 'H1', 'H2', 'G1']) if (!l.includes(x)) return `missing ${x} in: ${l}`;
});
await test('full-form JSON: deeply nested (30 levels)', async p => {
  await select(p, 1); await elementJson(p);
  await p.locator('.bd-json-mode-button').click(); await settle(p);
  const m = JSON.parse(await p.locator('.bd-json-editor').inputValue()); delete m.Parameters.HeaderModel; m.DynamicType.TypeDefinitionName = '';
  let node = { DynamicType: { ComponentType: 'String', TypeDefinitionName: '' }, Parameters: { Label: 'deep', Path: '$.deep' } };
  for (let i = 0; i < 30; i++) node = { DynamicType: { ComponentType: 'VerticalLayout', TypeDefinitionName: '' }, Elements: [node] };
  m.Elements = [node];
  await applyJson(p, JSON.stringify(m)); await refresh(p);
  if (!(await labels(p)).includes('deep')) return 'deep field not rendered';
});
await test('full-form JSON: no elements (empty form)', async p => {
  await select(p, 1); await elementJson(p);
  await p.locator('.bd-json-mode-button').click(); await settle(p);
  const m = JSON.parse(await p.locator('.bd-json-editor').inputValue()); delete m.Parameters.HeaderModel; m.DynamicType.TypeDefinitionName = '';
  m.Elements = [];
  await applyJson(p, JSON.stringify(m));
  if (await p.locator('.bd-json-error').count()) return 'error shown for empty form';
});

// ---------- type-specific props ----------
async function loadSingle(p, type, extra = {}) {
  await select(p, 1); await elementJson(p);
  await p.locator('.bd-json-mode-button').click(); await settle(p);
  const m = JSON.parse(await p.locator('.bd-json-editor').inputValue()); delete m.Parameters.HeaderModel; m.DynamicType.TypeDefinitionName = '';
  m.Elements = [{ DynamicType: { ComponentType: 'VerticalLayout', TypeDefinitionName: '' }, Elements: [{ DynamicType: { ComponentType: type, TypeDefinitionName: '' }, Parameters: { Label: 'X', Path: '$.x', ...extra } }] }];
  await applyJson(p, JSON.stringify(m)); await refresh(p);
  const cls = { Int: 'intcomponent', Boolean: 'boolcomponent', Dropdown: 'dropdowncomponent' }[type];
  await p.locator('.' + cls).first().dispatchEvent('click'); await settle(p);
}
await test('Boolean: Required checkbox persists', async p => {
  await loadSingle(p, 'Boolean');
  const cb = p.locator('.bd-property-editor input[type=checkbox]');
  if (!(await cb.count())) return 'no Required checkbox';
  await cb.check(); await settle(p);
  const j = await elementJson(p);
  if (!/"Required": true/.test(j)) return 'Required not in JSON: ' + j;
});
await test('Int: Min/Max persist & render', async p => {
  await loadSingle(p, 'Int');
  const ins = p.locator('.bd-property-editor input[type=number]');
  if ((await ins.count()) !== 2) return 'min/max inputs: ' + (await ins.count());
  await ins.nth(0).fill('5'); await p.keyboard.press('Tab'); await settle(p);
  await ins.nth(1).fill('10'); await p.keyboard.press('Tab'); await settle(p);
  const j = await elementJson(p);
  if (!/"Min": 5/.test(j) || !/"Max": 10/.test(j)) return j;
});
await test('Int: Min greater than Max / non-numeric', async p => {
  await loadSingle(p, 'Int');
  const ins = p.locator('.bd-property-editor input[type=number]');
  await ins.nth(0).fill('100'); await p.keyboard.press('Tab'); await settle(p);
  await ins.nth(1).fill('1'); await p.keyboard.press('Tab'); await settle(p);
  const j = await elementJson(p);
  if (/"Min": 100(\.0)?[,\n]/.test(j) && /"Max": 1(\.0)?[,\n]/.test(j)) return 'accepts Min(100) > Max(1) without warning';
});
await test('Int: clearing Min removes it', async p => {
  await loadSingle(p, 'Int', { Min: 3 });
  const ins = p.locator('.bd-property-editor input[type=number]');
  await ins.nth(0).fill(''); await p.keyboard.press('Tab'); await settle(p);
  const j = await elementJson(p);
  if (/"Min"/.test(j)) return 'Min still present: ' + j;
});
await test('Dropdown: add/rename/remove options', async p => {
  await loadSingle(p, 'Dropdown', { Options: { a: 'Alpha', b: 'Beta' } });
  const rows = () => p.locator('.bd-option-row').count();
  if ((await rows()) !== 2) return 'initial rows ' + (await rows());
  await p.locator('.bd-add-option').click(); await settle(p);
  if ((await rows()) !== 3) return 'add failed ' + (await rows());
  await p.locator('.bd-option-label').first().fill('Renamed'); await p.keyboard.press('Tab'); await settle(p);
  await p.locator('.bd-remove-option').nth(1).click(); await settle(p);
  if ((await rows()) !== 2) return 'remove failed ' + (await rows());
  const j = await elementJson(p);
  if (!j.includes('Renamed')) return 'rename not in JSON: ' + j;
});
await test('Dropdown: add option 3x after removing middle (key collision)', async p => {
  await loadSingle(p, 'Dropdown', { Options: { option1: 'A', option2: 'B' } });
  await p.locator('.bd-remove-option').first().click(); await settle(p); // leaves option2 -> count 1
  await p.locator('.bd-add-option').click(); await settle(p);           // option2 collides!
  const n = await p.locator('.bd-option-row').count();
  if (n !== 2) return `rows after remove+add: ${n} (expected 2) — new option overwrote existing key`;
});
await test('Dropdown: rename key to existing key', async p => {
  await loadSingle(p, 'Dropdown', { Options: { a: 'Alpha', b: 'Beta' } });
  await p.locator('.bd-option-key').nth(0).fill('b'); await p.keyboard.press('Tab'); await settle(p);
  const n = await p.locator('.bd-option-row').count();
  if (n !== 2) return `rows ${n}: renaming key a→b silently destroyed an option`;
});
await test('Dropdown: rendered options reflect edits', async p => {
  await loadSingle(p, 'Dropdown', { Options: { a: 'Alpha' } });
  await p.locator('.bd-add-option').click(); await settle(p);
  await editable(p).nth(1).locator('.mud-select, .mud-input-control').first().click();
  await settle(p);
  const items = await p.locator('.mud-list-item, .mud-select-item, option').allInnerTexts();
  if (!items.join().includes('Option 2')) return 'options in dropdown: ' + items.join(',');
});

// ---------- delete ----------
await test('delete a field', async p => {
  const n = await rootChildren(p);
  await select(p, 1);
  await p.locator('.bd-delete-button').click(); await settle(p);
  if ((await rootChildren(p)) !== n - 1) return `children ${n} → ${await rootChildren(p)}`;
  if (await p.locator('.bd-property-editor-header:not(.bd-pe-plain)').count()) return 'selection not cleared after delete';
});
await test('delete all fields then add one back', async p => {
  for (let i = 0; i < 3; i++) { await select(p, 1); await p.locator('.bd-delete-button').click(); await settle(p); }
  const n = await rootChildren(p);
  if (n !== 0) return 'remaining ' + n;
  const stale = await p.locator('.bd-empty').count();
  await refresh(p);
  if (!(await p.locator('.bd-empty').count())) return 'no drop placeholder in the emptied layout (cannot add fields back)';
  const sb = await p.locator('.bd-toolbox-item', { hasText: /^\s*Text Field\s*$/ }).boundingBox();
  const eb = await p.locator('.bd-empty').first().boundingBox();
  await mouseDrag(p, { x: sb.x + 20, y: sb.y + 10 }, { x: eb.x + eb.width / 2, y: eb.y + eb.height / 2 });
  if ((await rootChildren(p)) !== 1) return 'drop into emptied layout failed: ' + (await rootChildren(p));
  if (!stale) return 'placeholder missing right after deleting the last field (view stale until a re-render)';
});
await test('delete a layout removes its children too', async p => {
  await select(p, 0);
  await p.locator('.bd-delete-button').click(); await settle(p);
  const stale = await p.locator('.stringcomponent').count();
  await refresh(p);
  if ((await p.locator('.stringcomponent').count()) !== 0) return 'children remain even after re-render';
  if (stale) return `view stale after delete: ${stale} string fields still shown until a re-render`;
});

// ---------- drag & drop (real mouse) ----------
async function mouseDrag(p, from, toXY, { release = true, steps = 12 } = {}) {
  await p.mouse.move(from.x, from.y); await p.mouse.down();
  await p.mouse.move(from.x + 30, from.y + 30, { steps: 4 });
  await p.mouse.move(toXY.x, toXY.y, { steps }); await p.waitForTimeout(350);
  if (release) { await p.mouse.up(); await p.waitForTimeout(600); }
}
// slot k = "before child k"; slot n = after the last child of the root vertical layout
async function slotXY(p, k) {
  const n = await rootChildren(p);
  if (k < n) { const bb = await kids(p).nth(k).boundingBox(); return { x: bb.x + bb.width / 2, y: bb.y + 4 }; }
  const bb = await kids(p).nth(n - 1).boundingBox(); return { x: bb.x + bb.width / 2, y: bb.y + bb.height - 4 };
}
async function dragToolbox(p, name, slot) {
  const sb = await p.locator('.bd-toolbox-item', { hasText: new RegExp('^\\s*' + name + '\\s*$') }).boundingBox();
  await mouseDrag(p, { x: sb.x + sb.width / 2, y: sb.y + sb.height / 2 }, await slotXY(p, slot));
}
const visibleHandle = p => p.locator('.bd-tag').evaluateAll(ts => { const t = ts.find(t => getComputedStyle(t).opacity === '1'); if (!t) return null; const r = t.querySelector('.bd-handle').getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2 }; });
const order = p => p.locator('.bd-form-container-fullwidth label.mud-input-label').allTextContents();

await test('toolbox → form adds a Text Field at the dropped position', async p => {
  const n = await rootChildren(p);
  await dragToolbox(p, 'Text Field', 1);
  if ((await rootChildren(p)) !== n + 1) return `children ${n} → ${await rootChildren(p)}`;
  const o = await order(p);
  if (o[1] !== 'New Text Field') return 'landed at wrong position: ' + o.join(',');
});
for (const t of ['Checkbox', 'Integer', 'Number', 'Date/Time', 'Dropdown', 'Vertical Layout', 'Horizontal Layout', 'Group Layout', 'List']) {
  await test(`toolbox: add ${t}`, async p => {
    const n = await rootChildren(p);
    await dragToolbox(p, t, 1);
    if ((await rootChildren(p)) !== n + 1) return `children ${n} → ${await rootChildren(p)}`;
    await kids(p).nth(1).click({ position: { x: 5, y: 5 }, force: true }); await settle(p);
  });
}
await test('new element has a Path', async p => {
  await dragToolbox(p, 'Text Field', 0);
  await kids(p).nth(0).click({ position: { x: 5, y: 5 } }); await settle(p);
  const j = await elementJson(p);
  return /"Path"/.test(j) ? '' : 'no Path: ' + j.replace(/\s+/g, ' ');
});
await test('add two new fields → distinct paths', async p => {
  await dragToolbox(p, 'Text Field', 0);
  await dragToolbox(p, 'Text Field', 0);
  const paths = [];
  for (let i = 0; i < 2; i++) {
    await kids(p).nth(i).click({ position: { x: 5, y: 5 } }); await settle(p);
    paths.push(JSON.parse(await elementJson(p)).Parameters?.Path);
  }
  if (paths[0] === paths[1]) return 'duplicate default paths: ' + paths.join(',');
});
await test('reorder via handle (first → after second)', async p => {
  await kids(p).nth(0).hover(); await settle(p);
  const h = await visibleHandle(p);
  if (!h) return 'no visible handle on hover';
  const bb = await kids(p).nth(1).boundingBox();
  await mouseDrag(p, h, { x: bb.x + bb.width / 2, y: bb.y + bb.height - 4 });
  const o = await order(p);
  if (o[0] !== 'City' || o[1] !== 'Name') return 'order: ' + o.join(',');
});
await test('only one handle visible on hover; none when idle', async p => {
  const vis = () => p.locator('.bd-tag').evaluateAll(ts => ts.filter(t => getComputedStyle(t).opacity === '1').length);
  if (await vis()) return 'handles visible with no hover/selection';
  await kids(p).nth(1).hover(); await settle(p);
  const n = await vis(); if (n !== 1) return `${n} handles visible on hover`;
});
await test('drop targets show only while dragging, then disappear', async p => {
  if (await p.locator('.bd-zone').count()) return 'zones present while idle';
  const sb = await p.locator('.bd-toolbox-item', { hasText: /^\s*Checkbox\s*$/ }).boundingBox();
  await mouseDrag(p, { x: sb.x + 20, y: sb.y + 10 }, await slotXY(p, 1), { release: false });
  const during = await p.locator('.bd-zone').count(); const over = await p.locator('.bd-zone.over').count();
  await p.mouse.move(20, 20, { steps: 4 }); await p.mouse.up(); await p.waitForTimeout(600);
  const after = await p.locator('.bd-zone').count();
  if (!during) return 'no drop targets while dragging';
  if (!over) return 'hovered drop target not highlighted';
  if (after) return 'drop targets remain after drag ended';
});
await test('drag released outside leaves the form unchanged', async p => {
  const n = await rootChildren(p);
  const sb = await p.locator('.bd-toolbox-item', { hasText: /^\s*Checkbox\s*$/ }).boundingBox();
  await mouseDrag(p, { x: sb.x + 20, y: sb.y + 10 }, { x: 600, y: 60 });
  if ((await rootChildren(p)) !== n) return 'changed';
});
await test('layout dragged into its own child is rejected', async p => {
  await select(p, 1); await elementJson(p);
  await p.locator('.bd-json-mode-button').click(); await settle(p);
  const m = JSON.parse(await p.locator('.bd-json-editor').inputValue()); delete m.Parameters.HeaderModel; m.DynamicType.TypeDefinitionName = '';
  const f = (l, pa) => ({ DynamicType: { ComponentType: 'String', TypeDefinitionName: '' }, Parameters: { Label: l, Path: pa } });
  m.Elements = [{ DynamicType: { ComponentType: 'VerticalLayout', TypeDefinitionName: '' }, Elements: [{ DynamicType: { ComponentType: 'GroupLayout', TypeDefinitionName: '' }, Parameters: { Label: 'G' }, Elements: [f('Inner', '$.i'), f('Inner2', '$.i2')] }, f('Outer', '$.o')] }];
  await applyJson(p, JSON.stringify(m)); await refresh(p);
  await kids(p).nth(0).locator('.mud-card-header').hover(); await settle(p);
  const h = await visibleHandle(p);
  if (!h) return 'no handle on group';
  const inner = await p.locator('.stringcomponent').nth(1).boundingBox();
  await mouseDrag(p, h, { x: inner.x + inner.width / 2, y: inner.y + inner.height - 4 });
  const o = await order(p);
  if (o.join() !== 'Inner,Inner2,Outer') return 'order changed: ' + o.join();
  if (await p.locator('.groupcomponent .groupcomponent .groupcomponent').count()) return 'group nested inside itself';
});
await test('edit mode keeps field heights equal to view mode', async p => {
  const geo = () => p.evaluate(() => [...document.querySelectorAll('.stringcomponent')].map(e => Math.round(e.getBoundingClientRect().height)).join());
  const hEdit = await geo();
  await p.locator('.bd-toggle-slider').click(); await settle(p);
  const ex = p.getByRole('button', { name: 'Expand' }); if (await ex.count()) { await ex.click(); await settle(p); }
  const hView = await geo();
  if (hEdit !== hView) return `heights differ: edit ${hEdit} view ${hView}`;
});

// ---------- edit mode & toolbox ----------
await test('toggle Edit Mode off hides toolbox/handles, on restores', async p => {
  const tog = p.locator('.bd-toggle-slider');
  await tog.click(); await settle(p);
  if (await p.locator('.bd-toolbox-item').count() || await p.locator('.bd-drag-handle').count()) return 'editor chrome still visible when off';
  const collapsed = await p.getByRole('button', { name: 'Expand' }).count();
  if (collapsed) return 'group collapses again after toggling edit mode (expand state lost; fields hidden)';
  await tog.click(); await settle(p);
  if (!(await p.locator('.bd-toolbox-item').count())) return 'toolbox not back';
});
await test('edit mode off clears selection', async p => {
  await select(p, 1);
  await p.locator('.bd-toggle-slider').click(); await settle(p);
  await p.locator('.bd-toggle-slider').click(); await settle(p);
  if (await p.locator('.bd-property-editor-header:not(.bd-pe-plain)').count()) return 'selection survived edit-mode toggle (stale)';
});
await test('toolbox hide/show', async p => {
  await p.locator('.bd-toolbox-toggle').click(); await settle(p);
  if (await p.locator('.bd-toolbox-item').count()) return 'not hidden';
  await p.locator('.bd-toolbox-toggle').click(); await settle(p);
  if (!(await p.locator('.bd-toolbox-item').count())) return 'not shown';
});
await test('edit mode: typing in a field still works (data panel)', async p => {
  await editable(p).nth(1).locator('input').first().fill('typed'); await p.keyboard.press('Tab'); await settle(p);
  const body = (await p.waitForFunction(() => window.monaco && monaco.editor.getEditors().length, null, { timeout: 15000 }), await p.evaluate(() => monaco.editor.getEditors().map(e => e.getValue()).join('\n')));
  if (!body.includes('typed')) return 'value lost';
});
await test('typed value survives property edit of another field', async p => {
  await editable(p).nth(1).locator('input').first().fill('keepme'); await p.keyboard.press('Tab'); await settle(p);
  await select(p, 2);
  await p.locator('input.bd-property-input').first().fill('Other'); await p.keyboard.press('Tab'); await settle(p);
  const v = await editable(p).nth(1).locator('input').first().inputValue();
  if (v !== 'keepme') return `value "${v}"`;
});

// ---------- stress ----------
await test('rapid select/deselect 40x', async p => {
  for (let i = 0; i < 40; i++) { await editable(p).nth(1 + (i % 3)).click({ position: { x: 5, y: 5 } }); }
  await settle(p);
});
await test('rapid add 15 fields', async p => {
  for (let i = 0; i < 15; i++) await dragToolbox(p, 'Text Field', 0);
  const n = await rootChildren(p);
  if (n !== 18) return 'children ' + n;
});
await test('circuit stays alive after everything (reconnect modal absent)', async p => {
  await select(p, 1);
  if (await p.locator('#components-reconnect-modal.components-reconnect-show, .components-reconnect-show').count()) return 'reconnect modal visible';
});

await browser.close();
const bad = results.filter(r => r.status !== 'PASS');
console.log(`\n${results.length - bad.length}/${results.length} passed`);
