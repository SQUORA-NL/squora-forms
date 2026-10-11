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
      className="ff-action ff-delete-action"
      disabled={disabled}
      onClick={onDelete}
    >
      {label}
    </button>
  );
}
