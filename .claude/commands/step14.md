Final verification pass before GitHub submission.

Run each check in order and fix anything that fails before moving on:

1. Backend build:
   dotnet build
   → Must complete with 0 errors, 0 warnings

2. Tests:
   dotnet test TradeBlotter_Interview.Tests
   → All tests must pass

3. Frontend lint:
   cd frontend && npm run lint
   → Zero errors

4. Frontend production build:
   cd frontend && npm run build
   → Must complete successfully

5. Manual smoke test (run both backend and frontend, open http://localhost:5173):
   a. Submit a trade (e.g. Buy 100 AAPL @ 189.50) — blotter updates instantly, no reload
   b. Submit a second trade for the same symbol — position shows weighted average cost
   c. Submit a Sell for same symbol matching total qty — symbol disappears from positions panel
   d. Try submitting empty form — all four validation errors appear inline
   e. Confirm Buy rows show green badge, Sell rows show red badge in blotter
   f. Click Timestamp column header — sort reverses, indicator flips

6. Final commit:
   git add -A
   git commit -m "feat: complete trade blotter — .NET 8 API + Vue 3 frontend"

7. Save this entire Claude conversation:
   - Create a /claude-transcript/ folder in the repo
   - Export or copy the Claude chat history into it as a .txt or .md file
   - git add claude-transcript/ && git commit -m "docs: add claude transcript"
   - git push

Share the public GitHub repo link with the evaluators.
