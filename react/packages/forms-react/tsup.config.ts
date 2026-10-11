import { defineConfig } from 'tsup';

export default defineConfig({
  entry: ['src/index.ts'],
  format: ['esm', 'cjs'],
  dts: true,
  sourcemap: true,
  clean: true,
  external: ['react', 'react-dom', 'ajv'],
  // Components use hooks/state; mark the bundle as a client module for Next.js App Router
  banner: { js: '"use client";' },
});
