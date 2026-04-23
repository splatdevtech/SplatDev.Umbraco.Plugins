import { BaseConfig, BaseOutput, BaseUserConfig, BaseUserOutput, Plugin, PluginConfigMap, PluginNames, UserPostProcessor } from "@hey-api/shared";
import { TsConfigJsonResolved } from "get-tsconfig";
import { AnyString } from "@hey-api/types";

//#region src/config/output/postprocess.d.ts
/**
 * @deprecated Use `PostProcessorPreset` instead.
 */
type Formatters = 'biome' | 'prettier';
/**
 * @deprecated Use `PostProcessorPreset` instead.
 */
type Linters = 'biome' | 'eslint' | 'oxlint';
declare const postProcessors: {
  readonly 'biome:format': {
    readonly args: readonly ["format", "--write", "{{path}}"];
    readonly command: "biome";
    readonly name: "Biome (Format)";
  };
  readonly 'biome:lint': {
    readonly args: readonly ["lint", "--apply", "{{path}}"];
    readonly command: "biome";
    readonly name: "Biome (Lint)";
  };
  readonly eslint: {
    readonly args: readonly ["{{path}}", "--fix"];
    readonly command: "eslint";
    readonly name: "ESLint";
  };
  readonly oxfmt: {
    readonly args: readonly ["{{path}}"];
    readonly command: "oxfmt";
    readonly name: "Oxfmt";
  };
  readonly oxlint: {
    readonly args: readonly ["--fix", "{{path}}"];
    readonly command: "oxlint";
    readonly name: "Oxlint";
  };
  readonly prettier: {
    readonly args: readonly ["--ignore-unknown", "{{path}}", "--write", "--ignore-path", "./.prettierignore"];
    readonly command: "prettier";
    readonly name: "Prettier";
  };
};
type PostProcessorPreset = keyof typeof postProcessors;
//#endregion
//#region src/config/output/types.d.ts
type UserOutput = BaseUserOutput<'.js' | '.ts'> & {
  /**
   * Which formatter to use to process output folder?
   *
   * @default null
   * @deprecated Use `postProcess` instead.
   */
  format?: Formatters | null;
  /**
   * If specified, this will be the file extension used when importing
   * other modules. By default, we don't add a file extension and let the
   * runtime resolve it. If you're using moduleResolution `nodenext` or
   * `node16`, we default to `.js`.
   *
   * @default undefined
   * @deprecated Use `module.extension` instead.
   */
  importFileExtension?: '.js' | '.ts' | AnyString | null;
  /**
   * Which linter to use to process output folder?
   *
   * @default null
   * @deprecated Use `postProcess` instead.
   */
  lint?: Linters | null;
  /**
   * Post-processing commands to run on the output folder, executed in order.
   *
   * Use preset strings for common tools, or provide custom configurations.
   *
   * @example ['biome:lint', 'prettier']
   * @example [{ command: 'dprint', args: ['fmt', '{{path}}'] }]
   * @example ['eslint', { command: 'prettier', args: ['{{path}}', '--write'] }]
   *
   * @default []
   */
  postProcess?: ReadonlyArray<PostProcessorPreset | UserPostProcessor>;
  /**
   * Whether `export * from 'module'` should be used when possible
   * instead of named exports.
   *
   * @default false
   */
  preferExportAll?: boolean;
  /**
   * Relative or absolute path to the tsconfig file we should use to
   * generate the output. If a path to tsconfig file is not provided, we
   * attempt to find one starting from the location of the
   * `@hey-api/openapi-ts` configuration file and traversing up.
   *
   * @default undefined
   */
  tsConfigPath?: AnyString | null;
};
type Output = BaseOutput<'.js' | '.ts'> & {
  /**
   * Which formatter to use to process output folder?
   */
  format: Formatters | null;
  /**
   * Which linter to use to process output folder?
   */
  lint: Linters | null;
  /**
   * Whether `export * from 'module'` should be used when possible
   * instead of named exports.
   */
  preferExportAll: boolean;
  /**
   * The parsed TypeScript configuration used to generate the output.
   * If no `tsconfig` file path was provided or found, this will be `null`.
   */
  tsConfig: TsConfigJsonResolved | null;
  /**
   * Relative or absolute path to the tsconfig file we should use to
   * generate the output. If a path to tsconfig file is not provided, we
   * attempt to find one starting from the location of the
   * `@hey-api/openapi-ts` configuration file and traversing up.
   */
  tsConfigPath: AnyString | null | undefined;
};
//#endregion
//#region src/config/types.d.ts
type UserConfig = BaseUserConfig<UserOutput> & {
  /**
   * Plugins generate artifacts from `input`. By default, we generate SDK
   * functions and TypeScript interfaces. If you manually define `plugins`,
   * you need to include the default plugins if you wish to use them.
   *
   * @default ['@hey-api/typescript', '@hey-api/sdk']
   */
  plugins?: ReadonlyArray<PluginNames | { [K in PluginNames]: PluginConfigMap[K]['config'] & {
    name: K;
  } }[PluginNames]>;
};
type Config = BaseConfig<UserConfig, Output> & {
  pluginOrder: ReadonlyArray<keyof PluginConfigMap>;
  plugins: { [K in PluginNames]?: Plugin.Config<PluginConfigMap[K]> };
};
//#endregion
export { UserConfig as n, Config as t };
//# sourceMappingURL=types-DAEl4_a4.d.mts.map