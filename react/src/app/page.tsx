'use client';

import { useState, useCallback, useEffect } from 'react';
import { SquoraForm } from '@/lib/squora-forms';
import type { JSONSchema, DynamicFormModel } from '@/lib/squora-forms';

import contactSchema from '@/samples/contact-schema.json';
import contactUiSchema from '@/samples/contact-ui-schema.json';
import registrationSchema from '@/samples/registration-schema.json';
import registrationUiSchema from '@/samples/registration-ui-schema.json';
import completeSchema from '@/samples/complete-schema.json';
import completeUiSchema from '@/samples/complete-ui-schema.json';
import completeData from '@/samples/complete-data.json';

type TabId = 'paste' | 'contact' | 'registration' | 'complete';
type ThemeId = 'default' | 'dark' | 'compact' | 'studio' | 'glass' | 'neu' | 'vintage' | 'card';

const TABS: { id: TabId; label: string }[] = [
  { id: 'paste', label: 'Paste JSON' },
  { id: 'contact', label: 'Contact' },
  { id: 'registration', label: 'Registration' },
  { id: 'complete', label: 'Complete' },
];

const THEMES: { id: ThemeId; label: string }[] = [
  { id: 'default', label: 'Default' },
  { id: 'dark', label: 'Dark' },
  { id: 'compact', label: 'Compact' },
  { id: 'studio', label: 'Studio' },
  { id: 'glass', label: 'Glass' },
  { id: 'neu', label: 'Neu' },
  { id: 'vintage', label: 'Vintage' },
  { id: 'card', label: 'Card' },
];

export default function DemoPage() {
  const [activeTab, setActiveTab] = useState<TabId>('complete');
  const [theme, setTheme] = useState<ThemeId>('default');
  const [submittedData, setSubmittedData] = useState<Record<string, unknown> | null>(null);
  const [liveValues, setLiveValues] = useState<Record<string, unknown>>({});
  const [liveErrors, setLiveErrors] = useState<Record<string, string[]>>({});

  // Paste JSON state
  const [pasteSchemaText, setPasteSchemaText] = useState('');
  const [pasteUiSchemaText, setPasteUiSchemaText] = useState('');
  const [pasteDataText, setPasteDataText] = useState('');
  const [pasteError, setPasteError] = useState('');
  const [pastedSchema, setPastedSchema] = useState<JSONSchema | null>(null);
  const [pastedUiSchema, setPastedUiSchema] = useState<DynamicFormModel | null>(null);
  const [pastedData, setPastedData] = useState<Record<string, unknown> | undefined>(undefined);

  const [formKey, setFormKey] = useState(0);

  // Apply theme to document
  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
  }, [theme]);

  const handleSubmit = useCallback((data: Record<string, unknown>) => {
    setSubmittedData(data);
  }, []);

  const handleChange = useCallback(
    (data: Record<string, unknown>, errors: Record<string, string[]>) => {
      setLiveValues(data);
      setLiveErrors(errors);
    },
    []
  );

  const handleTabChange = useCallback((tab: TabId) => {
    setActiveTab(tab);
    setSubmittedData(null);
    setLiveValues({});
    setLiveErrors({});
    setFormKey((k) => k + 1);
  }, []);

  const handleRenderPasted = useCallback(() => {
    setPasteError('');
    try {
      const s = JSON.parse(pasteSchemaText);
      const u = JSON.parse(pasteUiSchemaText);
      const d = pasteDataText.trim() ? JSON.parse(pasteDataText) : undefined;
      setPastedSchema(s);
      setPastedUiSchema(u);
      setPastedData(d);
      setSubmittedData(null);
      setFormKey((k) => k + 1);
    } catch (err) {
      setPasteError(`Invalid JSON: ${err instanceof Error ? err.message : String(err)}`);
    }
  }, [pasteSchemaText, pasteUiSchemaText, pasteDataText]);

  function getFormProps(): {
    schema: JSONSchema;
    uiSchema: DynamicFormModel;
    initialValues?: Record<string, unknown>;
  } | null {
    switch (activeTab) {
      case 'contact':
        return { schema: contactSchema as JSONSchema, uiSchema: contactUiSchema as unknown as DynamicFormModel };
      case 'registration':
        return { schema: registrationSchema as JSONSchema, uiSchema: registrationUiSchema as unknown as DynamicFormModel };
      case 'complete':
        return { schema: completeSchema as JSONSchema, uiSchema: completeUiSchema as unknown as DynamicFormModel, initialValues: completeData as Record<string, unknown> };
      case 'paste':
        if (pastedSchema && pastedUiSchema) return { schema: pastedSchema, uiSchema: pastedUiSchema, initialValues: pastedData };
        return null;
      default:
        return null;
    }
  }

  const formProps = getFormProps();

  return (
    <div className="demo-root">
      {/* Sticky header — FIX-007: fixed layout, no content-driven shifts */}
      <div className="demo-header">
        <div className="demo-header-row">
          <div>
            <h1 style={{ margin: 0, fontSize: '22px' }}>SquoraForm React Demo</h1>
            <p style={{ color: 'var(--ff-fg)', opacity: 0.6, marginTop: '2px', fontSize: '13px' }}>
              Full parity with Blazor showcases. Same definitions, zero conversion.
            </p>
          </div>
          <select
            className="theme-selector"
            value={theme}
            onChange={(e) => setTheme(e.target.value as ThemeId)}
          >
            {THEMES.map((t) => (
              <option key={t.id} value={t.id}>{t.label}</option>
            ))}
          </select>
        </div>

        <div className="demo-tabs">
          {TABS.map((tab) => (
            <button
              key={tab.id}
              type="button"
              className="demo-tab"
              data-active={activeTab === tab.id}
              onClick={() => handleTabChange(tab.id)}
            >
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      {/* Content — CSS Grid with fixed columns, no reflow */}
      <div className="demo-content">
        <div className="demo-form-col">
          {/* Paste JSON inputs */}
          {activeTab === 'paste' && (
            <div style={{ marginBottom: '16px' }}>
              <div style={{ display: 'flex', gap: '12px', flexWrap: 'wrap', marginBottom: '8px' }}>
                <div style={{ flex: '1 1 300px' }}>
                  <label style={{ fontWeight: 600, fontSize: '13px' }}>JSON Schema</label>
                  <textarea
                    value={pasteSchemaText}
                    onChange={(e) => setPasteSchemaText(e.target.value)}
                    placeholder='{"type":"object","properties":{...}}'
                    style={{ width: '100%', height: '150px', fontFamily: 'monospace', fontSize: '12px', padding: '8px', border: '1px solid var(--ff-border)', borderRadius: 'var(--ff-radius)', background: 'var(--ff-field-bg)', color: 'var(--ff-fg)', resize: 'vertical' }}
                  />
                </div>
                <div style={{ flex: '1 1 300px' }}>
                  <label style={{ fontWeight: 600, fontSize: '13px' }}>UI Schema (DynamicFormModel)</label>
                  <textarea
                    value={pasteUiSchemaText}
                    onChange={(e) => setPasteUiSchemaText(e.target.value)}
                    placeholder='{"dynamicType":{"componentType":"VerticalLayout"},...}'
                    style={{ width: '100%', height: '150px', fontFamily: 'monospace', fontSize: '12px', padding: '8px', border: '1px solid var(--ff-border)', borderRadius: 'var(--ff-radius)', background: 'var(--ff-field-bg)', color: 'var(--ff-fg)', resize: 'vertical' }}
                  />
                </div>
              </div>
              <div style={{ marginBottom: '8px' }}>
                <label style={{ fontWeight: 600, fontSize: '13px' }}>Initial Data (optional)</label>
                <textarea
                  value={pasteDataText}
                  onChange={(e) => setPasteDataText(e.target.value)}
                  placeholder='{"firstName":"John",...}'
                  style={{ width: '100%', height: '60px', fontFamily: 'monospace', fontSize: '12px', padding: '8px', border: '1px solid var(--ff-border)', borderRadius: 'var(--ff-radius)', background: 'var(--ff-field-bg)', color: 'var(--ff-fg)', resize: 'vertical' }}
                />
              </div>
              <button
                type="button"
                onClick={handleRenderPasted}
                className="ff-submit-button"
              >
                Render Form
              </button>
              {pasteError && (
                <div style={{ marginTop: '8px', color: 'var(--ff-error)', fontSize: '13px', padding: '8px', backgroundColor: 'var(--ff-del-bg)', borderRadius: 'var(--ff-radius)' }}>
                  {pasteError}
                </div>
              )}
            </div>
          )}

          {/* Rendered form */}
          {formProps && (
            <div className="demo-form-panel">
              <SquoraForm
                key={`${activeTab}-${formKey}`}
                schema={formProps.schema}
                uiSchema={formProps.uiSchema}
                initialValues={formProps.initialValues}
                onSubmit={handleSubmit}
                onChange={handleChange}
              />
            </div>
          )}

          {activeTab === 'paste' && !formProps && (
            <div style={{ border: '1px dashed var(--ff-border)', borderRadius: '8px', padding: '40px', textAlign: 'center', color: 'var(--ff-fg)', opacity: 0.5 }}>
              Paste your JSON Schema and UI Schema above, then click &quot;Render Form&quot;.
            </div>
          )}
        </div>

        {/* Right: Output */}
        <div className="demo-output-col">
          <div className="demo-output-panel" style={{ backgroundColor: submittedData ? 'var(--ff-add-bg)' : undefined }}>
            <h3 style={{ margin: '0 0 8px 0', fontSize: '14px' }}>Submitted Data</h3>
            {submittedData ? (
              <pre>{JSON.stringify(submittedData, null, 2)}</pre>
            ) : (
              <p style={{ color: 'var(--ff-fg)', opacity: 0.5, margin: 0, fontStyle: 'italic', fontSize: '13px' }}>
                Submit the form to see data here.
              </p>
            )}
          </div>

          <div className="demo-output-panel">
            <h3 style={{ margin: '0 0 8px 0', fontSize: '14px' }}>Live Values</h3>
            <pre style={{ fontSize: '11px' }}>{JSON.stringify(liveValues, null, 2) || '{}'}</pre>
          </div>

          {Object.keys(liveErrors).length > 0 && (
            <div className="demo-output-panel" style={{ borderColor: 'var(--ff-error)', backgroundColor: 'var(--ff-del-bg)' }}>
              <h3 style={{ margin: '0 0 8px 0', color: 'var(--ff-error)', fontSize: '14px' }}>Errors</h3>
              <pre style={{ fontSize: '11px', color: 'var(--ff-error)' }}>{JSON.stringify(liveErrors, null, 2)}</pre>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
