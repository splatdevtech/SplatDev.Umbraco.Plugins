import { loadManifestApi } from './load-manifest-api.function.js';
/**
 * @param {UmbControllerHost} host - The controller host for this controller to be appended to
 * @param {ManifestApi} manifest - The manifest of the extension
 * @param {Array | UmbApiConstructorArgumentsMethodType} constructorArgs - The constructor arguments to pass to the API class
 * @param {ApiLoaderProperty} fallbackApi - A fallback API loader property to use if the manifest does not have one
 * @returns {Promise<UmbApi | undefined>} - The API class instance
 */
export async function createExtensionApi(host, manifest, constructorArgs, fallbackApi) {
    const apiPropValue = manifest.api ?? manifest.js ?? fallbackApi;
    if (!apiPropValue) {
        console.error(`-- Extension of alias "${manifest.alias}" did not succeed creating an API class instance, missing a JavaScript file via the 'api' or 'js' property, using either an 'api' or 'default' export.`, manifest);
        return undefined;
    }
    const apiConstructor = await loadManifestApi(apiPropValue);
    if (apiConstructor) {
        const additionalArgs = (typeof constructorArgs === 'function' ? constructorArgs(manifest) : constructorArgs) ?? [];
        return new apiConstructor(host, ...additionalArgs);
    }
    console.error(`-- Extension of alias "${manifest.alias}" did not succeed instantiating an API class instance via the extension manifest 'api' or 'js' property, using either an 'api' or 'default' export.`, manifest);
    return undefined;
}
