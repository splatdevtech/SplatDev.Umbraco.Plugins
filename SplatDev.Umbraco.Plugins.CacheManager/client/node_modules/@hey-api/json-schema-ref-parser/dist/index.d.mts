import { JSONSchema4, JSONSchema4Object, JSONSchema4Type, JSONSchema6, JSONSchema6Object, JSONSchema6Type, JSONSchema7, JSONSchema7Object, JSONSchema7Type } from "json-schema";

//#region src/types/index.d.ts
type JSONSchema = JSONSchema4 | JSONSchema6 | JSONSchema7;
type JSONSchemaObject = JSONSchema4Object | JSONSchema6Object | JSONSchema7Object;
interface Plugin {
  /**
   * Can this parser be used to process this file?
   */
  canHandle: (file: FileInfo) => boolean;
  /**
   * This is where the real work of a parser happens. The `parse` method accepts the same file info object as the `canHandle` function, but rather than returning a boolean value, the `parse` method should return a JavaScript representation of the file contents.  For our CSV parser, that is a two-dimensional array of lines and values.  For your parser, it might be an object, a string, a custom class, or anything else.
   *
   * Unlike the `canHandle` function, the `parse` method can also be asynchronous. This might be important if your parser needs to retrieve data from a database or if it relies on an external HTTP service to return the parsed value.  You can return your asynchronous value via a [Promise](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise) or a Node.js-style error-first callback.  Here are examples of both approaches:
   */
  handler: (file: FileInfo) => string | Buffer | JSONSchema | Promise<{
    data: Buffer;
  }> | Promise<string | Buffer | JSONSchema>;
  name: 'binary' | 'file' | 'http' | 'json' | 'text' | 'yaml';
}
/**
 * JSON Schema `$Ref` Parser supports plug-ins, such as resolvers and parsers. These plug-ins can have methods such as `canHandle()`, `read()`, `canHandle()`, and `parse()`. All of these methods accept the same object as their parameter: an object containing information about the file being read or parsed.
 *
 * The file info object currently only consists of a few properties, but it may grow in the future if plug-ins end up needing more information.
 *
 * See https://apitools.dev/json-schema-ref-parser/docs/plugins/file-info-object.html
 */
interface FileInfo {
  /**
   * The raw file contents, in whatever form they were returned by the resolver that read the file.
   */
  data: string | Buffer;
  /**
   * The lowercase file extension, such as ".json", ".yaml", ".txt", etc.
   */
  extension: string;
  /**
   * The hash (URL fragment) of the file URL, including the # symbol. If the URL doesn't have a hash, then this will be an empty string.
   */
  hash: string;
  /**
   * The full URL of the file. This could be any type of URL, including "http://", "https://", "file://", "ftp://", "mongodb://", or even a local filesystem path (when running in Node.js).
   */
  url: string;
}
//#endregion
//#region src/options.d.ts
interface DereferenceOptions {
  /**
   * Determines whether circular `$ref` pointers are handled.
   *
   * If set to `false`, then a `ReferenceError` will be thrown if the schema contains any circular references.
   *
   * If set to `"ignore"`, then circular references will simply be ignored. No error will be thrown, but the `$Refs.circular` property will still be set to `true`.
   */
  circular?: boolean | 'ignore';
  /**
   * A function, called for each path, which can return true to stop this path and all
   * subpaths from being dereferenced further. This is useful in schemas where some
   * subpaths contain literal $ref keys that should not be dereferenced.
   */
  excludedPathMatcher?(path: string): boolean;
  /**
   * Callback invoked during dereferencing.
   *
   * @argument {string} path - The path being dereferenced (ie. the `$ref` string)
   * @argument {JSONSchemaObject} value - The JSON-Schema that the `$ref` resolved to
   * @argument {JSONSchemaObject} parent - The parent of the dereferenced object
   * @argument {string} parentPropName - The prop name of the parent object whose value was dereferenced
   */
  onDereference?(path: string, value: JSONSchemaObject, parent?: JSONSchemaObject, parentPropName?: string): void;
}
/**
 * Options that determine how JSON schemas are parsed, resolved, and dereferenced.
 *
 * @param [options] - Overridden options
 * @class
 */
interface $RefParserOptions {
  /**
   * The `dereference` options control how JSON Schema `$Ref` Parser will dereference `$ref` pointers within the JSON schema.
   */
  dereference: DereferenceOptions;
  /**
   * The `parse` options determine how different types of files will be parsed.
   *
   * JSON Schema `$Ref` Parser comes with built-in JSON, YAML, plain-text, and binary parsers, any of which you can configure or disable. You can also add your own custom parsers if you want.
   */
  parse: {
    binary: Plugin;
    json: Plugin;
    text: Plugin;
    yaml: Plugin;
  };
  /**
   * The maximum amount of time (in milliseconds) that JSON Schema $Ref Parser will spend dereferencing a single schema.
   * It will throw a timeout error if the operation takes longer than this.
   */
  timeoutMs?: number;
}
type DeepPartial<T> = T extends object ? { [P in keyof T]?: DeepPartial<T[P]> } : T;
type ParserOptions = DeepPartial<$RefParserOptions>;
//#endregion
//#region src/util/errors.d.ts
type JSONParserErrorType = 'EUNKNOWN' | 'EPARSER' | 'EUNMATCHEDPARSER' | 'ETIMEOUT' | 'ERESOLVER' | 'EUNMATCHEDRESOLVER' | 'EMISSINGPOINTER' | 'EINVALIDPOINTER';
declare class JSONParserError extends Error {
  readonly name: string;
  readonly message: string;
  source: string | undefined;
  path: Array<string | number> | null;
  readonly code: JSONParserErrorType;
  constructor(message: string, source?: string);
  get footprint(): string;
}
declare class JSONParserErrorGroup<S extends object = JSONSchema> extends Error {
  files: $RefParser;
  constructor(parser: $RefParser);
  static getParserErrors<S extends object = JSONSchema>(parser: $RefParser): JSONParserError[];
  get errors(): Array<JSONParserError | InvalidPointerError | ResolverError | ParserError | MissingPointerError | UnmatchedParserError | UnmatchedResolverError>;
}
declare class ParserError extends JSONParserError {
  code: JSONParserErrorType;
  name: string;
  constructor(message: any, source: any);
}
declare class UnmatchedParserError extends JSONParserError {
  code: JSONParserErrorType;
  name: string;
  constructor(source: string);
}
declare class ResolverError extends JSONParserError {
  code: JSONParserErrorType;
  name: string;
  ioErrorCode?: string;
  constructor(ex: Error | any, source?: string);
}
declare class UnmatchedResolverError extends JSONParserError {
  code: JSONParserErrorType;
  name: string;
  constructor(source: any);
}
declare class MissingPointerError extends JSONParserError {
  code: JSONParserErrorType;
  name: string;
  constructor(token: string, path: string);
}
declare class TimeoutError extends JSONParserError {
  code: JSONParserErrorType;
  name: string;
  constructor(timeout: number);
}
declare class InvalidPointerError extends JSONParserError {
  code: JSONParserErrorType;
  name: string;
  constructor(pointer: string, path: string);
}
declare function isHandledError(err: any): err is JSONParserError;
declare function normalizeError(err: any): any;
//#endregion
//#region src/ref.d.ts
type $RefError = JSONParserError | ResolverError | ParserError | MissingPointerError;
/**
 * This class represents a single JSON reference and its resolved value.
 *
 * @class
 */
declare class $Ref<S extends object = JSONSchema> {
  /**
   * The file path or URL of the referenced file.
   * This path is relative to the path of the main JSON schema file.
   *
   * This path does NOT contain document fragments (JSON pointers). It always references an ENTIRE file.
   * Use methods such as {@link $Ref#get}, {@link $Ref#resolve}, and {@link $Ref#exists} to get
   * specific JSON pointers within the file.
   *
   * @type {string}
   */
  path: undefined | string;
  /**
   * The resolved value of the JSON reference.
   * Can be any JSON type, not just objects. Unknown file types are represented as Buffers (byte arrays).
   *
   * @type {?*}
   */
  value: any;
  /**
   * The {@link $Refs} object that contains this {@link $Ref} object.
   *
   * @type {$Refs}
   */
  $refs: $Refs<S>;
  /**
   * Indicates the type of {@link $Ref#path} (e.g., "file", "http", etc.)
   */
  pathType: string | unknown;
  /**
   * List of all errors. Undefined if no errors.
   */
  errors: Array<$RefError>;
  constructor($refs: $Refs<S>);
  /**
   * Pushes an error to errors array.
   *
   * @param err - The error to be pushed
   * @returns
   */
  addError(err: $RefError): void;
  /**
   * Determines whether the given JSON reference exists within this {@link $Ref#value}.
   *
   * @param path - The full path being resolved, optionally with a JSON pointer in the hash
   * @param options
   * @returns
   */
  exists(path: string, options?: ParserOptions): boolean;
  /**
   * Resolves the given JSON reference within this {@link $Ref#value} and returns the resolved value.
   *
   * @param path - The full path being resolved, optionally with a JSON pointer in the hash
   * @param options
   * @returns - Returns the resolved value
   */
  get(path: string, options?: ParserOptions): any;
  /**
   * Resolves the given JSON reference within this {@link $Ref#value}.
   *
   * @param path - The full path being resolved, optionally with a JSON pointer in the hash
   * @param options
   * @param friendlyPath - The original user-specified path (used for error messages)
   * @param pathFromRoot - The path of `obj` from the schema root
   * @returns
   */
  resolve(path: string, options?: ParserOptions, friendlyPath?: string, pathFromRoot?: string): Pointer<S>;
  /**
   * Sets the value of a nested property within this {@link $Ref#value}.
   * If the property, or any of its parents don't exist, they will be created.
   *
   * @param path - The full path of the property to set, optionally with a JSON pointer in the hash
   * @param value - The value to assign
   */
  set(path: string, value: any): void;
  /**
   * Determines whether the given value is a JSON reference.
   *
   * @param value - The value to inspect
   * @returns
   */
  static is$Ref(value: unknown): value is {
    $ref: string;
    length?: number;
  };
  /**
   * Determines whether the given value is an external JSON reference.
   *
   * @param value - The value to inspect
   * @returns
   */
  static isExternal$Ref(value: unknown): boolean;
  /**
   * Determines whether the given value is a JSON reference, and whether it is allowed by the options.
   *
   * @param value - The value to inspect
   * @param options
   * @returns
   */
  static isAllowed$Ref(value: unknown): true | undefined;
  /**
   * Determines whether the given value is a JSON reference that "extends" its resolved value.
   * That is, it has extra properties (in addition to "$ref"), so rather than simply pointing to
   * an existing value, this $ref actually creates a NEW value that is a shallow copy of the resolved
   * value, plus the extra properties.
   *
   * @example: {
     person: {
       properties: {
         firstName: { type: string }
         lastName: { type: string }
       }
     }
     employee: {
       properties: {
         $ref: #/person/properties
         salary: { type: number }
       }
     }
   }
   *  In this example, "employee" is an extended $ref, since it extends "person" with an additional
   *  property (salary).  The result is a NEW value that looks like this:
   *
   *  {
   *    properties: {
   *      firstName: { type: string }
   *      lastName: { type: string }
   *      salary: { type: number }
   *    }
   *  }
   *
   * @param value - The value to inspect
   * @returns
   */
  static isExtended$Ref(value: unknown): boolean;
  /**
   * Returns the resolved value of a JSON Reference.
   * If necessary, the resolved value is merged with the JSON Reference to create a new object
   *
   * @example: {
  person: {
    properties: {
      firstName: { type: string }
      lastName: { type: string }
    }
  }
  employee: {
    properties: {
      $ref: #/person/properties
      salary: { type: number }
    }
  }
  } When "person" and "employee" are merged, you end up with the following object:
   *
   *  {
   *    properties: {
   *      firstName: { type: string }
   *      lastName: { type: string }
   *      salary: { type: number }
   *    }
   *  }
   *
   * @param $ref - The JSON reference object (the one with the "$ref" property)
   * @param resolvedValue - The resolved value, which can be any type
   * @returns - Returns the dereferenced value
   */
  static dereference<S extends object = JSONSchema>($ref: $Ref<S>, resolvedValue: S): S;
}
//#endregion
//#region src/pointer.d.ts
/**
 * This class represents a single JSON pointer and its resolved value.
 *
 * @param $ref
 * @param path
 * @param [friendlyPath] - The original user-specified path (used for error messages)
 * @class
 */
declare class Pointer<S extends object = JSONSchema> {
  /**
   * The {@link $Ref} object that contains this {@link Pointer} object.
   */
  $ref: $Ref<S>;
  /**
   * The file path or URL, containing the JSON pointer in the hash.
   * This path is relative to the path of the main JSON schema file.
   */
  path: string;
  /**
   * The original path or URL, used for error messages.
   */
  originalPath: string;
  /**
   * The value of the JSON pointer.
   * Can be any JSON type, not just objects. Unknown file types are represented as Buffers (byte arrays).
   */
  value: any;
  /**
   * Indicates whether the pointer references itself.
   */
  circular: boolean;
  /**
   * The number of indirect references that were traversed to resolve the value.
   * Resolving a single pointer may require resolving multiple $Refs.
   */
  indirections: number;
  constructor($ref: $Ref<S>, path: string, friendlyPath?: string);
  /**
   * Resolves the value of a nested property within the given object.
   *
   * @param obj - The object that will be crawled
   * @param options
   * @param pathFromRoot - the path of place that initiated resolving
   *
   * @returns
   * Returns a JSON pointer whose {@link Pointer#value} is the resolved value.
   * If resolving this value required resolving other JSON references, then
   * the {@link Pointer#$ref} and {@link Pointer#path} will reflect the resolution path
   * of the resolved value.
   */
  resolve(obj: S, options?: ParserOptions, pathFromRoot?: string): this;
  /**
   * Sets the value of a nested property within the given object.
   *
   * @param obj - The object that will be crawled
   * @param value - the value to assign
   * @param options
   *
   * @returns
   * Returns the modified object, or an entirely new object if the entire object is overwritten.
   */
  set(obj: S, value: any, options?: ParserOptions): any;
  /**
   * Parses a JSON pointer (or a path containing a JSON pointer in the hash)
   * and returns an array of the pointer's tokens.
   * (e.g., "schema.json#/definitions/person/name" => ["definitions", "person", "name"])
   *
   * The pointer is parsed according to RFC 6901
   * {@link https://tools.ietf.org/html/rfc6901#section-3}
   *
   * @param path
   * @param [originalPath]
   * @returns
   */
  static parse(path: string, originalPath?: string): string[];
  /**
   * Creates a JSON pointer path, by joining one or more tokens to a base path.
   *
   * @param base - The base path (e.g., "schema.json#/definitions/person")
   * @param tokens - The token(s) to append (e.g., ["name", "first"])
   * @returns
   */
  static join(base: string, tokens: string | string[]): string;
}
//#endregion
//#region src/refs.d.ts
interface $RefsMap<S extends object = JSONSchema> {
  [url: string]: $Ref<S>;
}
/**
 * When you call the resolve method, the value that gets passed to the callback function (or Promise) is a $Refs object. This same object is accessible via the parser.$refs property of $RefParser objects.
 *
 * This object is a map of JSON References and their resolved values. It also has several convenient helper methods that make it easy for you to navigate and manipulate the JSON References.
 *
 * See https://apitools.dev/json-schema-ref-parser/docs/refs.html
 */
declare class $Refs<S extends object = JSONSchema> {
  /**
   * This property is true if the schema contains any circular references. You may want to check this property before serializing the dereferenced schema as JSON, since JSON.stringify() does not support circular references by default.
   *
   * See https://apitools.dev/json-schema-ref-parser/docs/refs.html#circular
   */
  circular: boolean;
  /**
   * Returns the paths/URLs of all the files in your schema (including the main schema file).
   *
   * See https://apitools.dev/json-schema-ref-parser/docs/refs.html#pathstypes
   *
   * @param types (optional) Optionally only return certain types of paths ("file", "http", etc.)
   */
  paths(...types: (string | string[])[]): string[];
  /**
   * Returns a map of paths/URLs and their correspond values.
   *
   * See https://apitools.dev/json-schema-ref-parser/docs/refs.html#valuestypes
   *
   * @param types (optional) Optionally only return values from certain locations ("file", "http", etc.)
   */
  values(...types: (string | string[])[]): S;
  /**
   * Returns `true` if the given path exists in the schema; otherwise, returns `false`
   *
   * See https://apitools.dev/json-schema-ref-parser/docs/refs.html#existsref
   *
   * @param $ref The JSON Reference path, optionally with a JSON Pointer in the hash
   */
  /**
   * Determines whether the given JSON reference exists.
   *
   * @param path - The path being resolved, optionally with a JSON pointer in the hash
   * @param [options]
   * @returns
   */
  exists(path: string, options: any): boolean;
  /**
   * Resolves the given JSON reference and returns the resolved value.
   *
   * @param path - The path being resolved, with a JSON pointer in the hash
   * @param [options]
   * @returns - Returns the resolved value
   */
  get(path: string, options?: ParserOptions): JSONSchema4Type | JSONSchema6Type | JSONSchema7Type;
  /**
   * Sets the value at the given path in the schema. If the property, or any of its parents, don't exist, they will be created.
   *
   * @param path The JSON Reference path, optionally with a JSON Pointer in the hash
   * @param value The value to assign. Can be anything (object, string, number, etc.)
   */
  set(path: string, value: JSONSchema4Type | JSONSchema6Type | JSONSchema7Type): void;
  /**
   * Returns the specified {@link $Ref} object, or undefined.
   *
   * @param path - The path being resolved, optionally with a JSON pointer in the hash
   * @returns
   * @protected
   */
  _get$Ref(path: string): $Ref<S> | undefined;
  /**
   * Creates a new {@link $Ref} object and adds it to this {@link $Refs} object.
   *
   * @param path  - The file path or URL of the referenced file
   */
  _add(path: string): $Ref<S>;
  /**
   * Resolves the given JSON reference.
   *
   * @param path - The path being resolved, optionally with a JSON pointer in the hash
   * @param pathFromRoot - The path of `obj` from the schema root
   * @param [options]
   * @returns
   * @protected
   */
  _resolve(path: string, pathFromRoot: string, options?: ParserOptions): Pointer<S> | null;
  /**
   * A map of paths/urls to {@link $Ref} objects
   *
   * @type {object}
   * @protected
   */
  _$refs: $RefsMap<S>;
  /**
   * The {@link $Ref} object that is the root of the JSON schema.
   *
   * @type {$Ref}
   * @protected
   */
  _root$Ref: $Ref<S>;
  constructor();
  /**
   * Returns the paths of all the files/URLs that are referenced by the JSON schema,
   * including the schema itself.
   *
   * @param [types] - Only return paths of the given types ("file", "http", etc.)
   * @returns
   */
  /**
   * Returns the map of JSON references and their resolved values.
   *
   * @param [types] - Only return references of the given types ("file", "http", etc.)
   * @returns
   */
  /**
   * Returns a POJO (plain old JavaScript object) for serialization as JSON.
   *
   * @returns {object}
   */
  toJSON: (...types: (string | string[])[]) => S;
}
//#endregion
//#region src/resolvers/url.d.ts
declare const sendRequest: ({
  fetchOptions,
  redirects,
  timeout,
  url
}: {
  fetchOptions?: RequestInit;
  redirects?: string[];
  timeout?: number;
  url: URL | string;
}) => Promise<{
  fetchOptions?: RequestInit;
  response: Response;
}>;
//#endregion
//#region src/index.d.ts
interface ResolvedInput {
  path: string;
  schema: string | JSONSchema | Buffer | Awaited<JSONSchema> | undefined;
  type: 'file' | 'json' | 'url';
}
declare function getResolvedInput({
  pathOrUrlOrSchema
}: {
  pathOrUrlOrSchema: JSONSchema | string | unknown;
}): ResolvedInput;
/**
 * This class parses a JSON schema, builds a map of its JSON references and their resolved values,
 * and provides methods for traversing, manipulating, and dereferencing those references.
 */
declare class $RefParser {
  /**
   * The resolved JSON references
   *
   * @type {$Refs}
   * @readonly
   */
  $refs: $Refs<JSONSchema>;
  options: $RefParserOptions;
  /**
   * The parsed (and possibly dereferenced) JSON schema object
   *
   * @type {object}
   * @readonly
   */
  schema: JSONSchema | null;
  schemaMany: JSONSchema[];
  schemaManySources: string[];
  sourcePathToPrefix: Map<string, string>;
  /**
   * Bundles all referenced files/URLs into a single schema that only has internal `$ref` pointers. This lets you split-up your schema however you want while you're building it, but easily combine all those files together when it's time to package or distribute the schema to other people. The resulting schema size will be small, since it will still contain internal JSON references rather than being fully-dereferenced.
   *
   * This also eliminates the risk of circular references, so the schema can be safely serialized using `JSON.stringify()`.
   *
   * See https://apitools.dev/json-schema-ref-parser/docs/ref-parser.html#bundleschema-options-callback
   *
   * @param pathOrUrlOrSchema A JSON Schema object, or the file path or URL of a JSON Schema file.
   */
  bundle({
    arrayBuffer,
    fetch,
    pathOrUrlOrSchema,
    resolvedInput
  }: {
    arrayBuffer?: ArrayBuffer;
    fetch?: RequestInit;
    pathOrUrlOrSchema: JSONSchema | string | unknown;
    resolvedInput?: ResolvedInput;
  }): Promise<JSONSchema>;
  /**
   * Bundles multiple roots (files/URLs/objects) into a single schema by creating a synthetic root
   * that references each input, resolving all externals, and then hoisting via the existing bundler.
   */
  bundleMany({
    arrayBuffer,
    fetch,
    pathOrUrlOrSchemas,
    resolvedInputs
  }: {
    arrayBuffer?: ArrayBuffer[];
    fetch?: RequestInit;
    pathOrUrlOrSchemas: Array<JSONSchema | string | unknown>;
    resolvedInputs?: ResolvedInput[];
  }): Promise<JSONSchema>;
  /**
   * Parses the given JSON schema.
   * This method does not resolve any JSON references.
   * It just reads a single file in JSON or YAML format, and parse it as a JavaScript object.
   *
   * @param pathOrUrlOrSchema A JSON Schema object, or the file path or URL of a JSON Schema file.
   * @returns - The returned promise resolves with the parsed JSON schema object.
   */
  parse({
    arrayBuffer,
    fetch,
    pathOrUrlOrSchema,
    resolvedInput: _resolvedInput
  }: {
    arrayBuffer?: ArrayBuffer;
    fetch?: RequestInit;
    pathOrUrlOrSchema: JSONSchema | string | unknown;
    resolvedInput?: ResolvedInput;
  }): Promise<{
    schema: JSONSchema;
  }>;
  private parseMany;
  mergeMany(): JSONSchema;
}
//#endregion
export { $RefParser, InvalidPointerError, JSONParserError, JSONParserErrorGroup, type JSONParserErrorType, type JSONSchema, MissingPointerError, ParserError, ResolverError, TimeoutError, UnmatchedParserError, UnmatchedResolverError, getResolvedInput, isHandledError, normalizeError, sendRequest };
//# sourceMappingURL=index.d.mts.map