import { n as UserConfig, t as Config } from "./types-DAEl4_a4.mjs";
import { Logger } from "@hey-api/codegen-core";
import { ConfigError, getSpec, parseOpenApiSpec } from "@hey-api/shared";

//#region src/config/resolve.d.ts
type ResolvedJob = {
  config: Config;
  errors: Array<Error>;
  index: number;
};
//#endregion
//#region src/config/init.d.ts
type Configs = {
  dependencies: Record<string, string>;
  jobs: ReadonlyArray<ResolvedJob>;
  /**
   * @deprecated Use `jobs` instead.
   */
  results: ReadonlyArray<ResolvedJob>;
};
/**
 * @internal
 */
declare function resolveJobs({
  logger,
  userConfigs
}: {
  logger: Logger;
  userConfigs: ReadonlyArray<UserConfig>;
}): Promise<Configs>;
//#endregion
export { getSpec, resolveJobs as initConfigs, parseOpenApiSpec };
//# sourceMappingURL=internal.d.mts.map