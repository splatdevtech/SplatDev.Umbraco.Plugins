export const UMB_DEFAULT_LOCALIZATION_CULTURE = 'en';
export class UmbLocalizationManager {
    #documentElementObserver;
    #changedKeys;
    #requestUpdateChangedKeysId;
    get fallback() {
        return this.localizations.get(UMB_DEFAULT_LOCALIZATION_CULTURE);
    }
    constructor() {
        this.connectedControllers = new Set();
        this.#changedKeys = new Set();
        this.#requestUpdateChangedKeysId = undefined;
        this.localizations = new Map();
        this.documentDirection = document.documentElement.dir || 'ltr';
        this.documentLanguage = document.documentElement.lang || navigator.language;
        this.#registerLocalizationBind = this.registerLocalization.bind(this);
        /** Updates all localized elements that are currently connected */
        this.updateAll = () => {
            const newDir = document.documentElement.dir || 'ltr';
            const newLang = document.documentElement.lang || navigator.language;
            if (this.documentDirection === newDir && this.documentLanguage === newLang)
                return;
            // The document direction or language did changed, so lets move on:
            this.documentDirection = newDir;
            this.documentLanguage = newLang;
            // Check if there was any changed.
            this.connectedControllers.forEach((ctrl) => {
                ctrl.documentUpdate();
            });
            if (this.#requestUpdateChangedKeysId) {
                cancelAnimationFrame(this.#requestUpdateChangedKeysId);
                this.#requestUpdateChangedKeysId = undefined;
            }
            this.#changedKeys.clear();
        };
        this.#updateChangedKeys = () => {
            this.#requestUpdateChangedKeysId = undefined;
            this.connectedControllers.forEach((ctrl) => {
                ctrl.keysChanged(this.#changedKeys);
            });
            this.#changedKeys.clear();
        };
        this.#documentElementObserver = new MutationObserver(this.updateAll);
        this.#documentElementObserver.observe(document.documentElement, {
            attributes: true,
            attributeFilter: ['dir', 'lang'],
        });
    }
    appendConsumer(consumer) {
        if (this.connectedControllers.has(consumer))
            return;
        this.connectedControllers.add(consumer);
    }
    removeConsumer(consumer) {
        this.connectedControllers.delete(consumer);
    }
    /**
     * Registers one or more translations
     * @param t
     */
    registerLocalization(t) {
        const code = t.$code.toLowerCase();
        if (this.localizations.has(code)) {
            // Merge translations that share the same language code
            this.localizations.set(code, { ...this.localizations.get(code), ...t });
        }
        else {
            this.localizations.set(code, t);
        }
        // Declare what keys have been changed:
        const keys = Object.keys(t);
        for (const key of keys) {
            this.#changedKeys.add(key);
        }
        this.#requestChangedKeysUpdate();
    }
    #registerLocalizationBind;
    registerManyLocalizations(translations) {
        translations.map(this.#registerLocalizationBind);
    }
    #updateChangedKeys;
    /**
     * Request an update of all consumers of the keys defined in #changedKeys.
     * This waits one frame, which ensures that multiple changes are collected into one.
     */
    #requestChangedKeysUpdate() {
        if (this.#requestUpdateChangedKeysId)
            return;
        this.#requestUpdateChangedKeysId = requestAnimationFrame(this.#updateChangedKeys);
    }
}
export const umbLocalizationManager = new UmbLocalizationManager();
