import { defineConfig } from "vitest/config";

export default defineConfig({
  test: {
    root: __dirname,
    environment: "node",
    include: ["tests/**/*.spec.ts"],
    setupFiles: [
      "./vitest.setup.ts",
      "tests/_helpers.ts"
    ],
    coverage: {
      provider: "v8",
      reportsDirectory: "./coverage",
      reporter: ["text", "html"],
      include: ["src/**/*.ts"]
    }
  }
});
