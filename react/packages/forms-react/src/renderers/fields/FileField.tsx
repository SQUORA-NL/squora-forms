'use client';

import { useState } from 'react';
import type { FieldRendererProps } from '../../core/types';

/** A chosen file as written to the form data. `data` is the Base64 encoded content. */
export interface UploadedFile {
  name: string;
  contentType: string;
  size: number;
  data: string;
}

const DEFAULT_MAX_FILE_SIZE = 5 * 1024 * 1024;

function toFiles(value: unknown): UploadedFile[] {
  if (Array.isArray(value)) return value as UploadedFile[];
  return value && typeof value === 'object' ? [value as UploadedFile] : [];
}

function formatSize(bytes: number): string {
  if (bytes >= 1024 * 1024) return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  if (bytes >= 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${bytes} B`;
}

/** Reads a File into the { name, contentType, size, data } shape. */
export function readFile(file: File): Promise<UploadedFile> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onerror = () => reject(reader.error);
    reader.onload = () => {
      // The result is a data URL ("data:<type>;base64,<data>"): keep only the Base64 part.
      const result = String(reader.result);
      resolve({
        name: file.name,
        contentType: file.type,
        size: file.size,
        data: result.slice(result.indexOf(',') + 1),
      });
    };
    reader.readAsDataURL(file);
  });
}

export function FileField({
  name,
  value,
  model,
  schema,
  errors,
  disabled,
  onChange,
  onBlur,
}: FieldRendererProps) {
  const [localError, setLocalError] = useState<string | null>(null);

  const label = model.options?.label || (model.parameters?.label as string) || name;
  // The form definition wins; the JSON Schema (array of squora-file objects) is the fallback.
  const multiple = (model.parameters?.multiple as boolean | undefined) ?? schema.type === 'array';
  const fileSchema = (schema.type === 'array' ? schema.items : schema) as Record<string, unknown> | undefined;
  const accept = (model.parameters?.accept as string | undefined) ?? (fileSchema?.accept as string | undefined);
  const maxFileSize =
    (model.parameters?.maximumFileSize as number | undefined) ??
    (fileSchema?.maxFileSize as number | undefined) ??
    DEFAULT_MAX_FILE_SIZE;
  const maxFiles =
    (model.parameters?.maximumFiles as number | undefined) ||
    (schema.type === 'array' ? (schema as { maxItems?: number }).maxItems : undefined);

  const files = toFiles(value);
  const errorId = `${name}-error`;
  const allErrors = localError ? [localError, ...errors] : errors;
  const hasErrors = allErrors.length > 0;

  const commit = (next: UploadedFile[]) => {
    if (multiple) onChange(next.length > 0 ? next : undefined);
    else onChange(next[0]);
  };

  const handleChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const picked = Array.from(e.target.files ?? []);
    e.target.value = ''; // allow picking the same file again after removing it
    setLocalError(null);

    const accepted: UploadedFile[] = [];
    let error: string | null = null;
    for (const file of picked) {
      if (file.size > maxFileSize) {
        error = `${file.name} is larger than ${formatSize(maxFileSize)}.`;
        continue;
      }
      try {
        accepted.push(await readFile(file));
      } catch {
        error = `${file.name} could not be read.`;
      }
    }

    if (accepted.length > 0) {
      let next = multiple ? [...files, ...accepted] : [accepted[accepted.length - 1]];
      if (multiple && maxFiles && next.length > maxFiles) {
        error = `At most ${maxFiles} files are allowed.`;
        next = next.slice(0, maxFiles);
      }
      commit(next);
    }
    setLocalError(error);
    onBlur();
  };

  const remove = (index: number) => {
    setLocalError(null);
    commit(files.filter((_, i) => i !== index));
    onBlur();
  };

  return (
    <div className="sq-field sq-file-field">
      <label htmlFor={name}>{label}</label>
      <input
        type="file"
        id={name}
        name={name}
        multiple={multiple}
        accept={accept}
        disabled={disabled}
        aria-invalid={hasErrors}
        aria-describedby={hasErrors ? errorId : undefined}
        onChange={handleChange}
      />
      {files.length > 0 && (
        <ul className="sq-file-list">
          {files.map((file, index) => (
            <li key={`${file.name}-${index}`}>
              <span className="sq-file-name">{file.name}</span>{' '}
              <span className="sq-file-size">({formatSize(file.size)})</span>
              <button
                type="button"
                className="sq-file-remove"
                disabled={disabled}
                aria-label={`Remove ${file.name}`}
                onClick={() => remove(index)}
              >
                ×
              </button>
            </li>
          ))}
        </ul>
      )}
      {hasErrors && (
        <span id={errorId} className="sq-error" role="alert">
          {allErrors.join(', ')}
        </span>
      )}
    </div>
  );
}
