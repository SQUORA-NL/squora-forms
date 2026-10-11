'use client';

export interface AddActionProps {
  label: string;
  visible: boolean;
  disabled: boolean;
  onAdd: () => void;
}

export function AddAction({ label, visible, disabled, onAdd }: AddActionProps) {
  if (!visible) return null;

  return (
    <button
      type="button"
      className="sq-action sq-add-action"
      disabled={disabled}
      onClick={onAdd}
    >
      {label}
    </button>
  );
}
