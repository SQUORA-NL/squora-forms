/**
 * Form state management — useReducer-based.
 */

'use client';

import { useReducer, useCallback } from 'react';
import type { FormState, FormAction } from './types';
import { appendToArray, removeFromArray, setValueAtPath } from './scope';

function createInitialState(
  initialValues?: Record<string, unknown>
): FormState {
  return {
    values: initialValues ? { ...initialValues } : {},
    errors: {},
    touched: {},
  };
}

export function formReducer(state: FormState, action: FormAction): FormState {
  switch (action.type) {
    case 'CHANGE':
      return {
        ...state,
        values: setValueAtPath(
          state.values,
          action.payload.name,
          action.payload.value
        ),
      };

    case 'TOUCH':
      return {
        ...state,
        touched: {
          ...state.touched,
          [action.payload.name]: true,
        },
      };

    case 'SET_ERRORS':
      return {
        ...state,
        errors: action.payload.errors,
      };

    case 'SUBMIT': {
      // Mark all fields as touched
      const allTouched: Record<string, boolean> = {};
      for (const key of Object.keys(state.values)) {
        allTouched[key] = true;
      }
      // Also mark any field with errors as touched
      for (const key of Object.keys(state.errors)) {
        allTouched[key] = true;
      }
      return {
        ...state,
        touched: { ...state.touched, ...allTouched },
      };
    }

    case 'ARRAY_ADD':
      return {
        ...state,
        values: appendToArray(
          state.values,
          action.payload.path,
          action.payload.defaultValue
        ),
      };

    case 'ARRAY_REMOVE':
      return {
        ...state,
        values: removeFromArray(
          state.values,
          action.payload.path,
          action.payload.index
        ),
      };

    case 'RESET':
      return createInitialState(action.payload?.values);

    default:
      return state;
  }
}

export function useFormState(initialValues?: Record<string, unknown>) {
  const [state, dispatch] = useReducer(
    formReducer,
    initialValues,
    createInitialState
  );

  const changeField = useCallback(
    (name: string, value: unknown) => {
      dispatch({ type: 'CHANGE', payload: { name, value } });
    },
    []
  );

  const touchField = useCallback((name: string) => {
    dispatch({ type: 'TOUCH', payload: { name } });
  }, []);

  const setErrors = useCallback(
    (errors: Record<string, string[]>) => {
      dispatch({ type: 'SET_ERRORS', payload: { errors } });
    },
    []
  );

  const submitForm = useCallback(() => {
    dispatch({ type: 'SUBMIT' });
  }, []);

  const resetForm = useCallback(
    (values?: Record<string, unknown>) => {
      dispatch({ type: 'RESET', payload: values ? { values } : undefined });
    },
    []
  );

  const addToArray = useCallback(
    (path: string, defaultValue: unknown) => {
      dispatch({ type: 'ARRAY_ADD', payload: { path, defaultValue } });
    },
    []
  );

  const removeFromArrayAction = useCallback(
    (path: string, index: number) => {
      dispatch({ type: 'ARRAY_REMOVE', payload: { path, index } });
    },
    []
  );

  return {
    state,
    changeField,
    touchField,
    setErrors,
    submitForm,
    resetForm,
    addToArray,
    removeFromArrayAction,
  };
}
