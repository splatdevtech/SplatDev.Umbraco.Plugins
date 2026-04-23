/**
 * @class UmbContextToken
 * @template BaseType - A generic type of the API before decimated.
 * @template ResultType - A concrete type of the API after decimation, use this when you apply a discriminator method. Note this is optional and defaults to the BaseType.
 */
export class UmbContextToken {
    #discriminator;
    /**
     * @param contextAlias   	Unique identifier for the context
     * @param apiAlias   			Unique identifier for the api
     * @param discriminator   A discriminator that will be used to discriminate the API — testing if the API lives up to a certain requirement. If the API does not meet the requirement then the consumer will not receive this API.
     */
    constructor(contextAlias, apiAlias = 'default', discriminator) {
        this.contextAlias = contextAlias;
        this.apiAlias = apiAlias;
        /**
         * Get the type of the token
         * @public
         * @type      {T}
         * @memberOf  UmbContextToken
         * @example   `typeof MyToken.TYPE`
         * @returns   undefined
         */
        this.TYPE = undefined;
        this.#discriminator = discriminator;
    }
    /**
     * Get the discriminator method for the token
     * @returns the discriminator method
     */
    getDiscriminator() {
        return this.#discriminator;
    }
    /**
     * This method must always return the unique alias of the token since that
     * will be used to look up the token in the injector.
     * @returns the unique alias of the token
     */
    toString() {
        return this.contextAlias + '#' + this.apiAlias;
    }
}
