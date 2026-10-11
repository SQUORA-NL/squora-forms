'use client';

export interface DeleteActionProps {
  label: string;
  visible: boolean;
  disabled: boolean;
  onDelete: () => void;
}

export function DeleteAction({
  label,
  visible,
  disabled,
  onDelete,
}: DeleteActionProps) {
  if (!visible) return null;

  return (
    <button
      type="button"
      className="sq-action sq-delete-action"
      disabled={disabled}
      onClick={onDelete}
    >
      {label}
    </button>
  );
}
