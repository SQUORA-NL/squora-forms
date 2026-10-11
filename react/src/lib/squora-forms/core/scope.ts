/**
 * Scope path resolution — converts JSON reference paths to dot-notation field names.
 * Zero React dependencies.
 *
 * "#/properties/name"                    → "name"
 * "#/properties/address/properties/city" → "address.city"
 * "$.firstName"                          → "firstName"
 * "$.Address.City"                       → "Address.City"
 * "@.Make"                               → "@.Make" (relative — resolved at render time)
 * "@"                                    → "@" (relative — the item itself)
 */

/**
 * Check if a scope path is relative (starts with @).
 */
export function isRelativePath(scope: string): boolean {
  if (!scope) return false;
  return scope === '@' || scope.startsWith('@.');
}

/**
 * Resolve a relative scope (@.Make, @) to an absolute dot-notation path
 * given the current list item context.
 */
export function resolveRelativeScope(
  scope: string,
  basePath: string,
  index: number
): string {
  if (scope === '@') {
    return `${basePath}.${index}`;
  }
  if (scope.startsWith('@.')) {
    const rest = scope.slice(2);
    return `${basePath}.${index}.${rest}`;
  }
  return scope;
}

/**
 * Convert a UI Schema scope string to a dot-notation field name.
 * Handles #/properties/x, $.x, and pass-through for @.x (relative).
 */
export function resolveScopePath(scope: string): string {
  if (!scope) return '';

  // Relative paths — return as-is (resolved at render time by List)
  if (isRelativePath(scope)) return scope;

  // Blazor $-notation: $.Address.City → Address.City
  if (scope.startsWith('$.')) return scope.slice(2);
  if (scope === '$') return '';

  // JSON pointer: #/properties/x/properties/y → x.y
  let path = scope.startsWith('#/') ? scope.slice(2) : scope;
  if (path.startsWith('#')) path = path.slice(1);

  const segments = path.split('/').filter((s) => s !== 'properties' && s !== '');
  return segments.join('.');
}

/**
 * Extract a value from a nested object using a dot-notation path.
 * Supports numeric segments for array indexing (e.g., "Cars.0.Make").
 */
export function getValueAtPath(
  data: Record<string, unknown>,
  path: string
): unknown {
  if (!path) return undefined;
  const segments = path.split('.');
  let current: unknown = data;
  for (const segment of segments) {
    if (current === null || current === undefined) return undefined;
    if (Array.isArray(current)) {
      const idx = Number(segment);
      if (Number.isNaN(idx)) return undefined;
      current = current[idx];
    } else if (typeof current === 'object') {
      current = (current as Record<string, unknown>)[segment];
    } else {
      return undefined;
    }
  }
  return current;
}

/**
 * Set a value in a nested object at a dot-notation path.
 * Supports numeric segments for array indexing.
 * Returns a new object (immutable).
 */
export function setValueAtPath(
  data: Record<string, unknown>,
  path: string,
  value: unknown
): Record<string, unknown> {
  if (!path) return data;
  const segments = path.split('.');
  if (segments.length === 1) {
    if (Array.isArray(data)) {
      const arr = [...data];
      arr[Number(segments[0])] = value;
      return arr as unknown as Record<string, unknown>;
    }
    return { ...data, [segments[0]]: value };
  }

  const [head, ...rest] = segments;
  const restPath = rest.join('.');

  if (Array.isArray(data)) {
    const arr = [...data];
    const idx = Number(head);
    const child = (arr[idx] ?? {}) as Record<string, unknown>;
    arr[idx] = setValueAtPath(child, restPath, value);
    return arr as unknown as Record<string, unknown>;
  }

  const child =
    data[head] !== null &&
    data[head] !== undefined &&
    typeof data[head] === 'object'
      ? Array.isArray(data[head])
        ? ([...(data[head] as unknown[])] as unknown as Record<string, unknown>)
        : (data[head] as Record<string, unknown>)
      : {};

  return {
    ...data,
    [head]: setValueAtPath(child, restPath, value),
  };
}

/**
 * Append an item to an array at the given dot-notation path.
 * Returns a new data object (immutable).
 */
export function appendToArray(
  data: Record<string, unknown>,
  path: string,
  defaultValue: unknown
): Record<string, unknown> {
  const arr = getValueAtPath(data, path);
  const current = Array.isArray(arr) ? arr : [];
  const clonedDefault =
    defaultValue !== null && typeof defaultValue === 'object'
      ? JSON.parse(JSON.stringify(defaultValue))
      : defaultValue;
  const newArr = [...current, clonedDefault];
  return setValueAtPath(data, path, newArr);
}

/**
 * Remove an item at the given index from an array at the given path.
 * Returns a new data object (immutable).
 */
export function removeFromArray(
  data: Record<string, unknown>,
  path: string,
  index: number
): Record<string, unknown> {
  const arr = getValueAtPath(data, path);
  if (!Array.isArray(arr)) return data;
  const newArr = arr.filter((_, i) => i !== index);
  return setValueAtPath(data, path, newArr);
}
