# Editor browser tests

End-to-end checks of the Blazor edit mode (drag and drop, selection, property editor, JSON editing), driven with real mouse input in headless Chromium.

```sh
# 1. start the MudBlazor sample, keeping its output (a dead circuit only shows up there)
cd samples/MudBlazorServerApp
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://localhost:5199 dotnet run --no-launch-profile > ../../tst/editor-e2e/app.log 2>&1 &

# 2. run
cd tst/editor-e2e && npm install && npx playwright install chromium && npm test
```

Two suites:

- `battle.mjs` (`npm run battle`): the property editor, JSON editing, every component type, drag and drop, on MudBlazor.
- `frameworks.mjs` (`npm run frameworks`): what the editor itself does (drop targets and what they say, selection, the action bar, the properties pane, click to add, switching library) once for each of MudBlazor, HTML and Radzen. `FRAMEWORKS=HTML,Radzen` picks which.

`npm test` runs both. `APP_URL`, `APP_LOG` and `OUT_DIR` override the defaults; `ONLY=<regex>` runs the tests whose name matches. Screenshots of failures land in `out/`.

Make sure the port is yours: another app already listening on it answers the tests instead of the one you just built.
