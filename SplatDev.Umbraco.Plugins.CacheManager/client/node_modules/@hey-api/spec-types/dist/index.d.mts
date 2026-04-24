import { AnyString, MaybeArray } from "@hey-api/types";

//#region src/extensions/code-samples.d.ts
type LinguistLanguages = 'C' | 'C#' | 'C++' | 'CoffeeScript' | 'CSS' | 'Dart' | 'DM' | 'Elixir' | 'Go' | 'Groovy' | 'HTML' | 'Java' | 'JavaScript' | 'Kotlin' | 'Objective-C' | 'Perl' | 'PHP' | 'PowerShell' | 'Python' | 'Ruby' | 'Rust' | 'Scala' | 'Shell' | 'Swift' | 'TypeScript';
interface CodeSampleObject {
  /**
   * Code sample label, for example `Node` or `Python2.7`.
   *
   * @default `lang` value
   */
  label?: string;
  /**
   * **REQUIRED**. Code sample language. Can be one of the automatically supported languages or any other language identifier of your choice (for custom code samples).
   */
  lang: LinguistLanguages;
  /**
   * **REQUIRED**. Code sample source code, or a `$ref` to the file containing the code sample.
   */
  source: string;
}
//#endregion
//#region src/extensions/enum.d.ts
interface EnumExtensions {
  /**
   * `x-enum-descriptions` are {@link https://stackoverflow.com/a/66471626 supported} by OpenAPI Generator.
   */
  'x-enum-descriptions'?: Array<string>;
  /**
   * `x-enum-varnames` are {@link https://stackoverflow.com/a/66471626 supported} by OpenAPI Generator.
   */
  'x-enum-varnames'?: Array<string>;
  /**
   * {@link https://github.com/RicoSuter/NSwag NSwag} generates `x-enumNames` field containing custom enum names.
   */
  'x-enumNames'?: Array<string>;
}
//#endregion
//#region src/extensions/spec.d.ts
/**
 * Specification Extensions.
 *
 * See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 */
interface SpecExtensions {
  [extension: `x-${string}`]: unknown;
}
/**
 * Type utility to wrap any type with specification extensions.
 */
type WithSpecExtensions<T> = T & SpecExtensions;
//#endregion
//#region src/json-schema/draft-4/spec.d.ts
interface Document$6 {
  /**
   * A schema can reference another schema using the `$ref` keyword. The value of `$ref` is a URI-reference that is resolved against the schema's {@link https://json-schema.org/understanding-json-schema/structuring#base-uri Base URI}. When evaluating a `$ref`, an implementation uses the resolved identifier to retrieve the referenced schema and applies that schema to the {@link https://json-schema.org/learn/glossary#instance instance}.
   *
   * The `$ref` keyword may be used to create recursive schemas that refer to themselves.
   */
  $ref?: string;
  /**
   * The `default` keyword specifies a default value. This value is not used to fill in missing values during the validation process. Non-validation tools such as documentation generators or form generators may use this value to give hints to users about how to use a value. However, `default` is typically used to express that if a value is missing, then the value is semantically the same as if the value was present with the default value. The value of `default` should validate against the schema in which it resides, but that isn't required.
   */
  default?: unknown;
  /**
   * The `title` and `description` keywords must be strings. A "title" will preferably be short, whereas a "description" will provide a more lengthy explanation about the purpose of the data described by the schema.
   */
  description?: string;
  /**
   * The `enum` {@link https://json-schema.org/learn/glossary#keyword keyword} is used to restrict a value to a fixed set of values. It must be an array with at least one element, where each element is unique.
   *
   * You can use `enum` even without a type, to accept values of different types.
   */
  enum?: Array<unknown>;
  /**
   * Ranges of numbers are specified using a combination of the `minimum` and `maximum` keywords, (or `exclusiveMinimum` and `exclusiveMaximum` for expressing exclusive range).
   *
   * If _x_ is the value being validated, the following must hold true:
   *
   * ```
   * x ≥ minimum
   * x > exclusiveMinimum
   * x ≤ maximum
   * x < exclusiveMaximum
   * ```
   *
   * While you can specify both of `minimum` and `exclusiveMinimum` or both of `maximum` and `exclusiveMaximum`, it doesn't really make sense to do so.
   */
  exclusiveMaximum?: boolean;
  /**
   * Ranges of numbers are specified using a combination of the `minimum` and `maximum` keywords, (or `exclusiveMinimum` and `exclusiveMaximum` for expressing exclusive range).
   *
   * If _x_ is the value being validated, the following must hold true:
   *
   * ```
   * x ≥ minimum
   * x > exclusiveMinimum
   * x ≤ maximum
   * x < exclusiveMaximum
   * ```
   *
   * While you can specify both of `minimum` and `exclusiveMinimum` or both of `maximum` and `exclusiveMaximum`, it doesn't really make sense to do so.
   */
  exclusiveMinimum?: boolean;
  /**
   * The `format` keyword allows for basic semantic identification of certain kinds of string values that are commonly used. For example, because JSON doesn't have a "DateTime" type, dates need to be encoded as strings. `format` allows the schema author to indicate that the string value should be interpreted as a date. By default, `format` is just an annotation and does not effect validation.
   *
   * Optionally, validator {@link https://json-schema.org/learn/glossary#implementation implementations} can provide a configuration option to enable `format` to function as an assertion rather than just an annotation. That means that validation will fail if, for example, a value with a `date` format isn't in a form that can be parsed as a date. This can allow values to be constrained beyond what the other tools in JSON Schema, including {@link https://json-schema.org/understanding-json-schema/reference/regular_expressions Regular Expressions} can do.
   *
   * There is a bias toward networking-related formats in the JSON Schema specification, most likely due to its heritage in web technologies. However, custom formats may also be used, as long as the parties exchanging the JSON documents also exchange information about the custom format types. A JSON Schema validator will ignore any format type that it does not understand.
   */
  format?: JsonSchemaFormats$2;
  /**
   * The length of the array can be specified using the `minItems` and `maxItems` keywords. The value of each keyword must be a non-negative number. These keywords work whether doing {@link https://json-schema.org/understanding-json-schema/reference/array#items list validation} or {@link https://json-schema.org/understanding-json-schema/reference/array#tupleValidation tuple-validation}.
   */
  maxItems?: number;
  /**
   * The length of a string can be constrained using the `minLength` and `maxLength` {@link https://json-schema.org/learn/glossary#keyword keywords}. For both keywords, the value must be a non-negative number.
   */
  maxLength?: number;
  /**
   * The number of properties on an object can be restricted using the `minProperties` and `maxProperties` keywords. Each of these must be a non-negative integer.
   */
  maxProperties?: number;
  /**
   * Ranges of numbers are specified using a combination of the `minimum` and `maximum` keywords, (or `exclusiveMinimum` and `exclusiveMaximum` for expressing exclusive range).
   *
   * If _x_ is the value being validated, the following must hold true:
   *
   * ```
   * x ≥ minimum
   * x > exclusiveMinimum
   * x ≤ maximum
   * x < exclusiveMaximum
   * ```
   *
   * While you can specify both of `minimum` and `exclusiveMinimum` or both of `maximum` and `exclusiveMaximum`, it doesn't really make sense to do so.
   */
  maximum?: number;
  /**
   * The length of the array can be specified using the `minItems` and `maxItems` keywords. The value of each keyword must be a non-negative number. These keywords work whether doing {@link https://json-schema.org/understanding-json-schema/reference/array#items list validation} or {@link https://json-schema.org/understanding-json-schema/reference/array#tupleValidation tuple-validation}.
   */
  minItems?: number;
  /**
   * The length of a string can be constrained using the `minLength` and `maxLength` {@link https://json-schema.org/learn/glossary#keyword keywords}. For both keywords, the value must be a non-negative number.
   */
  minLength?: number;
  /**
   * The number of properties on an object can be restricted using the `minProperties` and `maxProperties` keywords. Each of these must be a non-negative integer.
   */
  minProperties?: number;
  /**
   * Ranges of numbers are specified using a combination of the `minimum` and `maximum` keywords, (or `exclusiveMinimum` and `exclusiveMaximum` for expressing exclusive range).
   *
   * If _x_ is the value being validated, the following must hold true:
   *
   * ```
   * x ≥ minimum
   * x > exclusiveMinimum
   * x ≤ maximum
   * x < exclusiveMaximum
   * ```
   *
   * While you can specify both of `minimum` and `exclusiveMinimum` or both of `maximum` and `exclusiveMaximum`, it doesn't really make sense to do so.
   */
  minimum?: number;
  /**
   * Numbers can be restricted to a multiple of a given number, using the `multipleOf` keyword. It may be set to any positive number. The multiple can be a floating point number.
   */
  multipleOf?: number;
  /**
   * The `pattern` keyword is used to restrict a string to a particular regular expression. The regular expression syntax is the one defined in JavaScript ({@link https://www.ecma-international.org/publications-and-standards/standards/ecma-262/ ECMA 262} specifically) with Unicode support. See {@link https://json-schema.org/understanding-json-schema/reference/regular_expressions Regular Expressions} for more information.
   */
  pattern?: string;
  /**
   * By default, the properties defined by the `properties` keyword are not required. However, one can provide a list of required properties using the `required` keyword.
   *
   * The `required` keyword takes an array of zero or more strings. Each of these strings must be unique.
   */
  required?: Array<string>;
  /**
   * The `title` and `description` keywords must be strings. A "title" will preferably be short, whereas a "description" will provide a more lengthy explanation about the purpose of the data described by the schema.
   */
  title?: string;
  /**
   * Primitive data types in the Swagger Specification are based on the types supported by the {@link https://tools.ietf.org/html/draft-zyp-json-schema-04#section-3.5 JSON-Schema Draft 4}. Models are described using the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#schema-object Schema Object} which is a subset of JSON Schema Draft 4.
   */
  type?: JsonSchemaTypes$1;
  /**
   * A schema can ensure that each of the items in an array is unique. Simply set the `uniqueItems` keyword to `true`.
   */
  uniqueItems?: boolean;
}
type JsonSchemaFormats$2 = 'date-time' | 'email' | 'hostname' | 'ipv4' | 'ipv6' | 'uri' | AnyString;
type JsonSchemaTypes$1 = 'array' | 'boolean' | 'integer' | 'number' | 'object' | 'string';
declare namespace index_d_exports$1 {
  export { Document$6 as Document, JsonSchemaFormats$2 as JsonSchemaFormats, JsonSchemaTypes$1 as JsonSchemaTypes };
}
//#endregion
//#region src/json-schema/draft-2020-12/spec.d.ts
interface BaseDocument<TDocument = unknown> extends ArrayKeywords<TDocument>, NumberKeywords, ObjectKeywords<TDocument>, StringKeywords {
  /**
   * The `$comment` {@link https://json-schema.org/learn/glossary#keyword keyword} is strictly intended for adding comments to a schema. Its value must always be a string. Unlike the annotations `title`, `description`, and `examples`, JSON schema {@link https://json-schema.org/learn/glossary#implementation implementations} aren't allowed to attach any meaning or behavior to it whatsoever, and may even strip them at any time. Therefore, they are useful for leaving notes to future editors of a JSON schema, but should not be used to communicate to users of the schema.
   */
  $comment?: string;
  /**
   * A schema can reference another schema using the `$ref` keyword. The value of `$ref` is a URI-reference that is resolved against the schema's {@link https://json-schema.org/understanding-json-schema/structuring#base-uri Base URI}. When evaluating a `$ref`, an implementation uses the resolved identifier to retrieve the referenced schema and applies that schema to the {@link https://json-schema.org/learn/glossary#instance instance}.
   *
   * The `$ref` keyword may be used to create recursive schemas that refer to themselves.
   */
  $ref?: string;
  /**
   * `allOf`: (AND) Must be valid against _all_ of the {@link https://json-schema.org/learn/glossary#subschema subschemas}
   *
   * To validate against `allOf`, the given data must be valid against all of the given subschemas.
   *
   * {@link https://json-schema.org/understanding-json-schema/reference/combining#allof allOf} can not be used to "extend" a schema to add more details to it in the sense of object-oriented inheritance. {@link https://json-schema.org/learn/glossary#instance Instances} must independently be valid against "all of" the schemas in the `allOf`. See the section on {@link https://json-schema.org/understanding-json-schema/reference/object#extending Extending Closed Schemas} for more information.
   */
  allOf?: Array<TDocument>;
  /**
   * `anyOf`: (OR) Must be valid against _any_ of the subschemas
   *
   * To validate against `anyOf`, the given data must be valid against any (one or more) of the given subschemas.
   */
  anyOf?: Array<TDocument>;
  /**
   * The `const` keyword is used to restrict a value to a single value.
   */
  const?: unknown;
  /**
   * The `contentEncoding` keyword specifies the encoding used to store the contents, as specified in {@link https://tools.ietf.org/html/rfc2045 RFC 2054, part 6.1} and {@link https://datatracker.ietf.org/doc/html/rfc4648 RFC 4648}.
   *
   * The acceptable values are `quoted-printable`, `base16`, `base32`, and `base64`. If not specified, the encoding is the same as the containing JSON document.
   *
   * Without getting into the low-level details of each of these encodings, there are really only two options useful for modern usage:
   * - If the content is encoded in the same encoding as the enclosing JSON document (which for practical purposes, is almost always UTF-8), leave `contentEncoding` unspecified, and include the content in a string as-is. This includes text-based content types, such as `text/html` or `application/xml`.
   * - If the content is binary data, set `contentEncoding` to `base64` and encode the contents using {@link https://tools.ietf.org/html/rfc4648 Base64}. This would include many image types, such as `image/png` or audio types, such as `audio/mpeg`.
   */
  contentEncoding?: 'base16' | 'base32' | 'base64' | 'quoted-printable';
  /**
   * The `contentMediaType` keyword specifies the MIME type of the contents of a string, as described in {@link https://tools.ietf.org/html/rfc2046 RFC 2046}. There is a list of {@link http://www.iana.org/assignments/media-types/media-types.xhtml MIME types officially registered by the IANA}, but the set of types supported will be application and operating system dependent. Mozilla Developer Network also maintains a {@link https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Complete_list_of_MIME_types shorter list of MIME types that are important for the web}
   */
  contentMediaType?: string;
  /**
   * The `default` keyword specifies a default value. This value is not used to fill in missing values during the validation process. Non-validation tools such as documentation generators or form generators may use this value to give hints to users about how to use a value. However, `default` is typically used to express that if a value is missing, then the value is semantically the same as if the value was present with the default value. The value of `default` should validate against the schema in which it resides, but that isn't required.
   */
  default?: unknown;
  /**
   * The `dependentRequired` {@link https://json-schema.org/learn/glossary#keyword keyword} conditionally requires that certain properties must be present if a given property is present in an object. For example, suppose we have a {@link https://json-schema.org/learn/glossary#schema schema} representing a customer. If you have their credit card number, you also want to ensure you have a billing address. If you don't have their credit card number, a billing address would not be required. We represent this dependency of one property on another using the `dependentRequired` keyword. The value of the `dependentRequired` keyword is an object. Each entry in the object maps from the name of a property, _p_, to an array of strings listing properties that are required if _p_ is present.
   */
  dependentRequired?: Record<string, Array<string>>;
  /**
   * The `dependentSchemas` keyword conditionally applies a {@link https://json-schema.org/learn/glossary#subschema subschema} when a given property is present. This schema is applied in the same way {@link https://json-schema.org/understanding-json-schema/reference/combining#allof allOf} applies schemas. Nothing is merged or extended. Both schemas apply independently.
   */
  dependentSchemas?: Record<string, TDocument>;
  /**
   * The `deprecated` keyword is a boolean that indicates that the {@link https://json-schema.org/learn/glossary#instance instance} value the keyword applies to should not be used and may be removed in the future.
   */
  deprecated?: boolean;
  /**
   * The `title` and `description` keywords must be strings. A "title" will preferably be short, whereas a "description" will provide a more lengthy explanation about the purpose of the data described by the schema.
   */
  description?: string;
  /**
   * The `if`, `then` and `else` keywords allow the application of a subschema based on the outcome of another schema, much like the `if`/`then`/`else` constructs you've probably seen in traditional programming languages.
   *
   * If `if` is valid, `then` must also be valid (and `else` is ignored.) If `if` is invalid, `else` must also be valid (and `then` is ignored).
   *
   * If `then` or `else` is not defined, `if` behaves as if they have a value of `true`.
   *
   * If `then` and/or `else` appear in a schema without `if`, `then` and `else` are ignored.
   */
  else?: TDocument;
  /**
   * The `enum` {@link https://json-schema.org/learn/glossary#keyword keyword} is used to restrict a value to a fixed set of values. It must be an array with at least one element, where each element is unique.
   *
   * You can use `enum` even without a type, to accept values of different types.
   */
  enum?: Array<unknown>;
  /**
   * The `examples` keyword is a place to provide an array of examples that validate against the schema. This isn't used for validation, but may help with explaining the effect and purpose of the schema to a reader. Each entry should validate against the schema in which it resides, but that isn't strictly required. There is no need to duplicate the `default` value in the `examples` array, since `default` will be treated as another example.
   */
  examples?: Array<unknown>;
  /**
   * The `format` keyword allows for basic semantic identification of certain kinds of string values that are commonly used. For example, because JSON doesn't have a "DateTime" type, dates need to be encoded as strings. `format` allows the schema author to indicate that the string value should be interpreted as a date. By default, `format` is just an annotation and does not effect validation.
   *
   * Optionally, validator {@link https://json-schema.org/learn/glossary#implementation implementations} can provide a configuration option to enable `format` to function as an assertion rather than just an annotation. That means that validation will fail if, for example, a value with a `date` format isn't in a form that can be parsed as a date. This can allow values to be constrained beyond what the other tools in JSON Schema, including {@link https://json-schema.org/understanding-json-schema/reference/regular_expressions Regular Expressions} can do.
   *
   * There is a bias toward networking-related formats in the JSON Schema specification, most likely due to its heritage in web technologies. However, custom formats may also be used, as long as the parties exchanging the JSON documents also exchange information about the custom format types. A JSON Schema validator will ignore any format type that it does not understand.
   */
  format?: JsonSchemaFormats$1;
  /**
   * The `if`, `then` and `else` keywords allow the application of a subschema based on the outcome of another schema, much like the `if`/`then`/`else` constructs you've probably seen in traditional programming languages.
   *
   * If `if` is valid, `then` must also be valid (and `else` is ignored.) If `if` is invalid, `else` must also be valid (and `then` is ignored).
   *
   * If `then` or `else` is not defined, `if` behaves as if they have a value of `true`.
   *
   * If `then` and/or `else` appear in a schema without `if`, `then` and `else` are ignored.
   */
  if?: TDocument;
  /**
   * `not`: (NOT) Must _not_ be valid against the given schema
   *
   * The `not` keyword declares that an instance validates if it doesn't validate against the given subschema.
   */
  not?: TDocument;
  /**
   * `oneOf`: (XOR) Must be valid against _exactly one_ of the subschemas
   *
   * To validate against `oneOf`, the given data must be valid against exactly one of the given subschemas.
   *
   * Careful consideration should be taken when using `oneOf` entries as the nature of it requires verification of _every_ sub-schema which can lead to increased processing times. Prefer `anyOf` where possible.
   */
  oneOf?: Array<TDocument>;
  /**
   * The boolean keywords `readOnly` and `writeOnly` are typically used in an API context. `readOnly` indicates that a value should not be modified. It could be used to indicate that a `PUT` request that changes a value would result in a `400 Bad Request` response. `writeOnly` indicates that a value may be set, but will remain hidden. In could be used to indicate you can set a value with a `PUT` request, but it would not be included when retrieving that record with a `GET` request.
   */
  readOnly?: boolean;
  /**
   * The `if`, `then` and `else` keywords allow the application of a subschema based on the outcome of another schema, much like the `if`/`then`/`else` constructs you've probably seen in traditional programming languages.
   *
   * If `if` is valid, `then` must also be valid (and `else` is ignored.) If `if` is invalid, `else` must also be valid (and `then` is ignored).
   *
   * If `then` or `else` is not defined, `if` behaves as if they have a value of `true`.
   *
   * If `then` and/or `else` appear in a schema without `if`, `then` and `else` are ignored.
   */
  then?: TDocument;
  /**
   * The `title` and `description` keywords must be strings. A "title" will preferably be short, whereas a "description" will provide a more lengthy explanation about the purpose of the data described by the schema.
   */
  title?: string;
  /**
   * If it is an array, it must be an array of strings, where each string is the name of one of the basic types, and each element is unique. In this case, the JSON snippet is valid if it matches any of the given types.
   */
  type?: MaybeArray<JsonSchemaTypes>;
  /**
   * The boolean keywords `readOnly` and `writeOnly` are typically used in an API context. `readOnly` indicates that a value should not be modified. It could be used to indicate that a `PUT` request that changes a value would result in a `400 Bad Request` response. `writeOnly` indicates that a value may be set, but will remain hidden. In could be used to indicate you can set a value with a `PUT` request, but it would not be included when retrieving that record with a `GET` request.
   */
  writeOnly?: boolean;
}
type Document$5 = BaseDocument<Document$5>;
interface ArrayKeywords<TDocument = unknown> {
  /**
   * While the `items` schema must be valid for every item in the array, the `contains` schema only needs to validate against one or more items in the array.
   */
  contains?: TDocument;
  /**
   * List validation is useful for arrays of arbitrary length where each item matches the same schema. For this kind of array, set the `items` {@link https://json-schema.org/learn/glossary#keyword keyword} to a single schema that will be used to validate all of the items in the array.
   *
   * The `items` keyword can be used to control whether it's valid to have additional items in a tuple beyond what is defined in `prefixItems`. The value of the `items` keyword is a schema that all additional items must pass in order for the keyword to validate.
   *
   * Note that `items` doesn't "see inside" any {@link https://json-schema.org/learn/glossary#instance instances} of `allOf`, `anyOf`, or `oneOf` in the same {@link https://json-schema.org/learn/glossary#subschema subschema}.
   */
  items?: TDocument | false;
  /**
   * `minContains` and `maxContains` can be used with `contains` to further specify how many times a schema matches a `contains` constraint. These keywords can be any non-negative number including zero.
   */
  maxContains?: number;
  /**
   * The length of the array can be specified using the `minItems` and `maxItems` keywords. The value of each keyword must be a non-negative number. These keywords work whether doing {@link https://json-schema.org/understanding-json-schema/reference/array#items list validation} or {@link https://json-schema.org/understanding-json-schema/reference/array#tupleValidation tuple-validation}.
   */
  maxItems?: number;
  /**
   * `minContains` and `maxContains` can be used with `contains` to further specify how many times a schema matches a `contains` constraint. These keywords can be any non-negative number including zero.
   */
  minContains?: number;
  /**
   * The length of the array can be specified using the `minItems` and `maxItems` keywords. The value of each keyword must be a non-negative number. These keywords work whether doing {@link https://json-schema.org/understanding-json-schema/reference/array#items list validation} or {@link https://json-schema.org/understanding-json-schema/reference/array#tupleValidation tuple-validation}.
   */
  minItems?: number;
  /**
   * `prefixItems` is an array, where each item is a schema that corresponds to each index of the document's array. That is, an array where the first element validates the first element of the input array, the second element validates the second element of the input array, etc.
   */
  prefixItems?: Array<TDocument>;
  /**
   * The `unevaluatedItems` keyword is useful mainly when you want to add or disallow extra items to an array.
   *
   * `unevaluatedItems` applies to any values not evaluated by an `items`, `prefixItems`, or `contains` keyword. Just as `unevaluatedProperties` affects only properties in an object, `unevaluatedItems` affects only items in an array.
   *
   * Watch out! The word "unevaluated" _does not mean_ "not evaluated by `items`, `prefixItems`, or `contains`." "Unevaluated" means "not successfully evaluated", or "does not evaluate to true".
   *
   * Like with `items`, if you set `unevaluatedItems` to false, you can disallow extra items in the array.
   */
  unevaluatedItems?: TDocument | false;
  /**
   * A schema can ensure that each of the items in an array is unique. Simply set the `uniqueItems` keyword to `true`.
   */
  uniqueItems?: boolean;
}
interface NumberKeywords {
  /**
   * Ranges of numbers are specified using a combination of the `minimum` and `maximum` keywords, (or `exclusiveMinimum` and `exclusiveMaximum` for expressing exclusive range).
   *
   * If _x_ is the value being validated, the following must hold true:
   *
   * ```
   * x ≥ minimum
   * x > exclusiveMinimum
   * x ≤ maximum
   * x < exclusiveMaximum
   * ```
   *
   * While you can specify both of `minimum` and `exclusiveMinimum` or both of `maximum` and `exclusiveMaximum`, it doesn't really make sense to do so.
   */
  exclusiveMaximum?: number;
  /**
   * Ranges of numbers are specified using a combination of the `minimum` and `maximum` keywords, (or `exclusiveMinimum` and `exclusiveMaximum` for expressing exclusive range).
   *
   * If _x_ is the value being validated, the following must hold true:
   *
   * ```
   * x ≥ minimum
   * x > exclusiveMinimum
   * x ≤ maximum
   * x < exclusiveMaximum
   * ```
   *
   * While you can specify both of `minimum` and `exclusiveMinimum` or both of `maximum` and `exclusiveMaximum`, it doesn't really make sense to do so.
   */
  exclusiveMinimum?: number;
  /**
   * Ranges of numbers are specified using a combination of the `minimum` and `maximum` keywords, (or `exclusiveMinimum` and `exclusiveMaximum` for expressing exclusive range).
   *
   * If _x_ is the value being validated, the following must hold true:
   *
   * ```
   * x ≥ minimum
   * x > exclusiveMinimum
   * x ≤ maximum
   * x < exclusiveMaximum
   * ```
   *
   * While you can specify both of `minimum` and `exclusiveMinimum` or both of `maximum` and `exclusiveMaximum`, it doesn't really make sense to do so.
   */
  maximum?: number;
  /**
   * Ranges of numbers are specified using a combination of the `minimum` and `maximum` keywords, (or `exclusiveMinimum` and `exclusiveMaximum` for expressing exclusive range).
   *
   * If _x_ is the value being validated, the following must hold true:
   *
   * ```
   * x ≥ minimum
   * x > exclusiveMinimum
   * x ≤ maximum
   * x < exclusiveMaximum
   * ```
   *
   * While you can specify both of `minimum` and `exclusiveMinimum` or both of `maximum` and `exclusiveMaximum`, it doesn't really make sense to do so.
   */
  minimum?: number;
  /**
   * Numbers can be restricted to a multiple of a given number, using the `multipleOf` keyword. It may be set to any positive number. The multiple can be a floating point number.
   */
  multipleOf?: number;
}
interface ObjectKeywords<TDocument = unknown> {
  /**
   * The `additionalProperties` keyword is used to control the handling of extra stuff, that is, properties whose names are not listed in the `properties` keyword or match any of the regular expressions in the `patternProperties` keyword. By default any additional properties are allowed.
   *
   * The value of the `additionalProperties` keyword is a schema that will be used to validate any properties in the {@link https://json-schema.org/learn/glossary#instance instance} that are not matched by `properties` or `patternProperties`. Setting the `additionalProperties` schema to `false` means no additional properties will be allowed.
   *
   * It's important to note that `additionalProperties` only recognizes properties declared in the same {@link https://json-schema.org/learn/glossary#subschema subschema} as itself. So, `additionalProperties` can restrict you from "extending" a schema using {@link https://json-schema.org/understanding-json-schema/reference/combining combining} keywords such as {@link https://json-schema.org/understanding-json-schema/reference/combining#allof allOf}.
   */
  additionalProperties?: TDocument | false;
  /**
   * The number of properties on an object can be restricted using the `minProperties` and `maxProperties` keywords. Each of these must be a non-negative integer.
   */
  maxProperties?: number;
  /**
   * The number of properties on an object can be restricted using the `minProperties` and `maxProperties` keywords. Each of these must be a non-negative integer.
   */
  minProperties?: number;
  /**
   * Sometimes you want to say that, given a particular kind of property name, the value should match a particular schema. That's where `patternProperties` comes in: it maps regular expressions to schemas. If a property name matches the given regular expression, the property value must validate against the corresponding schema.
   */
  patternProperties?: Record<string, TDocument>;
  /**
   * The properties (key-value pairs) on an object are defined using the `properties` {@link https://json-schema.org/learn/glossary#keyword keyword}. The value of `properties` is an object, where each key is the name of a property and each value is a {@link https://json-schema.org/learn/glossary#schema schema} used to validate that property. Any property that doesn't match any of the property names in the `properties` keyword is ignored by this keyword.
   */
  properties?: Record<string, TDocument | true>;
  /**
   * The names of properties can be validated against a schema, irrespective of their values. This can be useful if you don't want to enforce specific properties, but you want to make sure that the names of those properties follow a specific convention. You might, for example, want to enforce that all names are valid ASCII tokens so they can be used as attributes in a particular programming language.
   *
   * Since object keys must always be strings anyway, it is implied that the schema given to `propertyNames` is always at least:
   *
   * ```json
   * { "type": "string" }
   * ```
   */
  propertyNames?: TDocument;
  /**
   * By default, the properties defined by the `properties` keyword are not required. However, one can provide a list of required properties using the `required` keyword.
   *
   * The `required` keyword takes an array of zero or more strings. Each of these strings must be unique.
   */
  required?: Array<string>;
  /**
   * The `unevaluatedProperties` keyword is similar to `additionalProperties` except that it can recognize properties declared in subschemas. So, the example from the previous section can be rewritten without the need to redeclare properties.
   *
   * `unevaluatedProperties` works by collecting any properties that are successfully validated when processing the schemas and using those as the allowed list of properties. This allows you to do more complex things like conditionally adding properties.
   */
  unevaluatedProperties?: TDocument | false;
}
interface StringKeywords {
  /**
   * The length of a string can be constrained using the `minLength` and `maxLength` {@link https://json-schema.org/learn/glossary#keyword keywords}. For both keywords, the value must be a non-negative number.
   */
  maxLength?: number;
  /**
   * The length of a string can be constrained using the `minLength` and `maxLength` {@link https://json-schema.org/learn/glossary#keyword keywords}. For both keywords, the value must be a non-negative number.
   */
  minLength?: number;
  /**
   * The `pattern` keyword is used to restrict a string to a particular regular expression. The regular expression syntax is the one defined in JavaScript ({@link https://www.ecma-international.org/publications-and-standards/standards/ecma-262/ ECMA 262} specifically) with Unicode support. See {@link https://json-schema.org/understanding-json-schema/reference/regular_expressions Regular Expressions} for more information.
   */
  pattern?: string;
}
type JsonSchemaFormats$1 = 'date' | 'date-time' | 'duration' | 'email' | 'hostname' | 'idn-email' | 'idn-hostname' | 'ipv4' | 'ipv6' | 'iri' | 'iri-reference' | 'json-pointer' | 'regex' | 'relative-json-pointer' | 'time' | 'uri' | 'uri-reference' | 'uri-template' | 'uuid' | AnyString;
type JsonSchemaTypes = 'array' | 'boolean' | 'integer' | 'null' | 'number' | 'object' | 'string';
declare namespace index_d_exports {
  export { ArrayKeywords, BaseDocument, Document$5 as Document, JsonSchemaFormats$1 as JsonSchemaFormats, JsonSchemaTypes, NumberKeywords, ObjectKeywords, StringKeywords };
}
declare namespace union_d_exports {
  export { Document$4 as Document };
}
/**
 * Union of all supported JSON Schema document versions.
 */
type Document$4 = Document$6 | Document$5;
//#endregion
//#region src/openapi/v2/extensions.d.ts
interface OpenAPIV2NullableExtensions {
  /**
   * OpenAPI 2.0 does not natively support null as a type, but you can use
   * `x-nullable` to polyfill this functionality.
   */
  'x-nullable'?: boolean;
}
//#endregion
//#region src/openapi/v2/spec.d.ts
/**
 * This is the root document object for the API specification. It combines what previously was the Resource Listing and API Declaration (version 1.2 and earlier) together into one document.
 */
interface Document$3 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * The base path on which the API is served, which is relative to the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerHost `host`}. If it is not included, the API is served directly under the `host`. The value MUST start with a leading slash (`/`). The `basePath` does not support {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#path-templating path templating}.
   */
  basePath?: string;
  /**
   * A list of MIME types the APIs can consume. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#mime-types Mime Types}.
   */
  consumes?: Array<string>;
  /**
   * An object to hold data types produced and consumed by operations.
   */
  definitions?: DefinitionsObject;
  /**
   * Additional external documentation.
   */
  externalDocs?: ExternalDocumentationObject$2;
  /**
   * The host (name or ip) serving the API. This MUST be the host only and does not include the scheme nor sub-paths. It MAY include a port. If the `host` is not included, the host serving the documentation is to be used (including the port). The `host` does not support {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#path-templating path templating}.
   */
  host?: string;
  /**
   * **Required**. Provides metadata about the API. The metadata can be used by the clients if needed.
   */
  info: InfoObject$2;
  /**
   * An object to hold parameters that can be used across operations. This property _does not_ define global parameters for all operations.
   */
  parameters?: ParametersDefinitionsObject;
  /**
   * **Required**. The available paths and operations for the API.
   */
  paths: PathsObject$2;
  /**
   * A list of MIME types the APIs can produce. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#mime-types Mime Types}.
   */
  produces?: Array<string>;
  /**
   * An object to hold responses that can be used across operations. This property _does not_ define global responses for all operations.
   */
  responses?: ResponsesDefinitionsObject;
  /**
   * The transfer protocol of the API. Values MUST be from the list: `"http"`, `"https"`, `"ws"`, `"wss"`. If the `schemes` is not included, the default scheme to be used is the one used to access the Swagger definition itself.
   */
  schemes?: Array<'http' | 'https' | 'ws' | 'wss'>;
  /**
   * A declaration of which security schemes are applied for the API as a whole. The list of values describes alternative security schemes that can be used (that is, there is a logical OR between the security requirements). Individual operations can override this definition.
   */
  security?: Array<SecurityRequirementObject$2>;
  /**
   * Security scheme definitions that can be used across the specification.
   */
  securityDefinitions?: SecurityDefinitionsObject;
  /**
   * **Required**. Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be `"2.0"`.
   */
  swagger: '2.0';
  /**
   * A list of tags used by the specification with additional metadata. The order of the tags can be used to reflect on their order by the parsing tools. Not all tags that are used by the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#operation-object Operation Object} must be declared. The tags that are not declared may be organized randomly or based on the tools' logic. Each tag name in the list MUST be unique.
   */
  tags?: Array<TagObject$2>;
}
/**
 * Contact information for the exposed API.
 *
 * @example
 * ```yaml
 * name: API Support
 * url: http://www.swagger.io/support
 * email: support@swagger.io
 * ```
 */
interface ContactObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * The email address of the contact person/organization. MUST be in the format of an email address.
   */
  email?: string;
  /**
   * The identifying name of the contact person/organization.
   */
  name?: string;
  /**
   * The URL pointing to the contact information. MUST be in the format of a URL.
   */
  url?: string;
}
/**
 * An object to hold data types that can be consumed and produced by operations. These data types can be primitives, arrays or models.
 *
 * **Definitions Object Example**
 *
 * @example
 * ```yaml
 * Category:
 *   type: object
 *   properties:
 *     id:
 *       type: integer
 *       format: int64
 *     name:
 *       type: string
 * Tag:
 *   type: object
 *   properties:
 *     id:
 *       type: integer
 *       format: int64
 *     name:
 *       type: string
 * ```
 */
interface DefinitionsObject {
  /**
   * A single definition, mapping a "name" to the schema it defines.
   */
  [name: string]: SchemaObject$2;
}
/**
 * Allows sharing examples for operation responses.
 *
 * **Example Object Example**
 *
 * Example response for application/json mimetype of a Pet data type:
 *
 * @example
 * ```yaml
 * application/json:
 *   name: Puma
 *   type: Dog
 *   color: Black
 *   gender: Female
 *   breed: Mixed
 * ```
 */
interface ExampleObject$2 {
  /**
   * The name of the property MUST be one of the Operation `produces` values (either implicit or inherited). The value SHOULD be an example of what such a response would look like.
   */
  [mimeType: string]: unknown;
}
/**
 * Allows referencing an external resource for extended documentation.
 *
 * @example
 * ```yaml
 * description: Find more info here
 * url: https://swagger.io
 * ```
 */
interface ExternalDocumentationObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * A short description of the target documentation. {@link https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown GFM syntax} can be used for rich text representation.
   */
  description?: string;
  /**
   * **Required**. The URL for the target documentation. Value MUST be in the format of a URL.
   */
  url: string;
}
/**
 * **Header Object Example**
 *
 * A simple header with of an integer type:
 *
 * @example
 * ```yaml
 * description: The number of allowed requests in the current period
 * type: integer
 * ```
 */
interface HeaderObject$2 extends EnumExtensions, OpenAPIV2NullableExtensions {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * Determines the format of the array if type array is used. Possible values are:
   *
   * - `csv` - comma separated values `foo,bar`.
   * - `ssv` - space separated values `foo bar`.
   * - `tsv` - tab separated values `foo\tbar`.
   * - `pipes` - pipe separated values `foo|bar`.
   *
   * Default value is `csv`.
   */
  collectionFormat?: 'csv' | 'pipes' | 'ssv' | 'tsv';
  /**
   * Declares the value of the item that the server will use if none is provided. (Note: "default" has no meaning for required items.) See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-6.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-6.2}. Unlike JSON Schema this value MUST conform to the defined {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#itemsType `type`} for the data type.
   */
  default?: unknown;
  /**
   * A short description of the header.
   */
  description?: string;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.5.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.5.1}.
   */
  enum?: Array<unknown>;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2}.
   */
  exclusiveMaximum?: boolean;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3}.
   */
  exclusiveMinimum?: boolean;
  /**
   * The extending format for the previously mentioned {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#stType `type`}. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#dataTypeFormat Data Type Formats} for further details.
   */
  format?: string;
  /**
   * **Required if {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterType `type`} is "array"**. Describes the type of items in the array.
   */
  items?: ItemsObject;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.2}.
   */
  maxItems?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.1}.
   */
  maxLength?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2}.
   */
  maximum?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.3}.
   */
  minItems?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.2}.
   */
  minLength?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3}.
   */
  minimum?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.1}.
   */
  multipleOf?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.3}.
   */
  pattern?: string;
  /**
   * Required. The type of the object. The value MUST be one of `"string"`, `"number"`, `"integer"`, `"boolean"`, or `"array"`.
   */
  type: 'array' | 'boolean' | 'integer' | 'number' | 'string';
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.4 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.4}.
   */
  uniqueItems?: boolean;
}
/**
 * Lists the headers that can be sent as part of a response.
 *
 * **Headers Object Example**
 *
 * Rate-limit headers:
 *
 * @example
 * ```yaml
 * X-Rate-Limit-Limit:
 *   description: The number of allowed requests in the current period
 *   type: integer
 * X-Rate-Limit-Remaining:
 *   description: The number of remaining requests in the current period
 *   type: integer
 * X-Rate-Limit-Reset:
 *   description: The number of seconds left in the current period
 *   type: integer
 * ```
 */
interface HeadersObject {
  /**
   * The name of the property corresponds to the name of the header. The value describes the type of the header.
   */
  [name: string]: HeaderObject$2;
}
/**
 * The object provides metadata about the API. The metadata can be used by the clients if needed, and can be presented in the Swagger-UI for convenience.
 *
 * @example
 * ```yaml
 * title: Swagger Sample App
 * description: This is a sample server Petstore server.
 * termsOfService: http://swagger.io/terms/
 * contact:
 *   name: API Support
 *   url: http://www.swagger.io/support
 *   email: support@swagger.io
 * license:
 *   name: Apache 2.0
 *   url: http://www.apache.org/licenses/LICENSE-2.0.html
 * version: 1.0.1
 * ```
 */
interface InfoObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * The contact information for the exposed API.
   */
  contact?: ContactObject$2;
  /**
   * A short description of the application. {@link https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown GFM syntax} can be used for rich text representation.
   */
  description?: string;
  /**
   * The license information for the exposed API.
   */
  license?: LicenseObject$2;
  /**
   * The Terms of Service for the API.
   */
  termsOfService?: string;
  /**
   * **Required**. The title of the application.
   */
  title: string;
  /**
   * **Required** Provides the version of the application API (not to be confused with the specification version).
   */
  version: string;
}
/**
 * A limited subset of JSON-Schema's items object. It is used by parameter definitions that are not located {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn `in`} `"body"`.
 *
 * **Items Object Examples**
 *
 * Items must be of type string and have the minimum length of 2 characters:
 *
 * @example
 * ```yaml
 * type: string
 * minLength: 2
 * ```
 *
 * An array of arrays, the internal array being of type integer, numbers must be between 0 and 63 (inclusive):
 *
 * @example
 * ```yaml
 * type: array
 * items:
 *   type: integer
 *   minimum: 0
 *   maximum: 63
 * ```
 */
interface ItemsObject extends EnumExtensions, OpenAPIV2NullableExtensions {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * Determines the format of the array if type array is used. Possible values are:
   *
   * - `csv` - comma separated values `foo,bar`.
   * - `ssv` - space separated values `foo bar`.
   * - `tsv` - tab separated values `foo\tbar`.
   * - `pipes` - pipe separated values `foo|bar`.
   *
   * Default value is `csv`.
   */
  collectionFormat?: 'csv' | 'pipes' | 'ssv' | 'tsv';
  /**
   * Declares the value of the item that the server will use if none is provided. (Note: "default" has no meaning for required items.) See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-6.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-6.2}. Unlike JSON Schema this value MUST conform to the defined {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#itemsType `type`} for the data type.
   */
  default?: unknown;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.5.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.5.1}.
   */
  enum?: Array<unknown>;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2}.
   */
  exclusiveMaximum?: boolean;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3}.
   */
  exclusiveMinimum?: boolean;
  /**
   * The extending format for the previously mentioned {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterType `type`}. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#dataTypeFormat Data Type Formats} for further details.
   */
  format?: string;
  /**
   * **Required if {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterType `type`} is "array"**. Describes the type of items in the array.
   */
  items?: ItemsObject;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.2}.
   */
  maxItems?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.1}.
   */
  maxLength?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2}.
   */
  maximum?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.3}.
   */
  minItems?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.2}.
   */
  minLength?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3}.
   */
  minimum?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.1}.
   */
  multipleOf?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.3}.
   */
  pattern?: string;
  /**
   * **Required**. The internal type of the array. The value MUST be one of `"string"`, `"number"`, `"integer"`, `"boolean"`, or `"array"`. Files and models are not allowed.
   */
  type: 'array' | 'boolean' | 'integer' | 'number' | 'string';
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.4 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.4}.
   */
  uniqueItems?: boolean;
}
/**
 * License information for the exposed API.
 *
 * @example
 * ```yaml
 * name: Apache 2.0
 * url: http://www.apache.org/licenses/LICENSE-2.0.html
 * ```
 */
interface LicenseObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * **Required**. The license name used for the API.
   */
  name: string;
  /**
   * A URL to the license used for the API. MUST be in the format of a URL.
   */
  url?: string;
}
/**
 * Describes a single API operation on a path.
 *
 * @example
 * ```yaml
 * tags:
 * - pet
 * summary: Updates a pet in the store with form data
 * description: ""
 * operationId: updatePetWithForm
 * consumes:
 * - application/x-www-form-urlencoded
 * produces:
 * - application/json
 * - application/xml
 * parameters:
 * - name: petId
 *   in: path
 *   description: ID of pet that needs to be updated
 *   required: true
 *   type: string
 * - name: name
 *   in: formData
 *   description: Updated name of the pet
 *   required: false
 *   type: string
 * - name: status
 *   in: formData
 *   description: Updated status of the pet
 *   required: false
 *   type: string
 * responses:
 *   '200':
 *     description: Pet updated.
 *   '405':
 *     description: Invalid input
 * security:
 * - petstore_auth:
 *   - write:pets
 *   - read:pets
 * ```
 */
interface OperationObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * A list of MIME types the operation can consume. This overrides the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerConsumes `consumes`} definition at the Swagger Object. An empty value MAY be used to clear the global definition. Value MUST be as described under {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#mime-types Mime Types}.
   */
  consumes?: Array<string>;
  /**
   * Declares this operation to be deprecated. Usage of the declared operation should be refrained. Default value is `false`.
   */
  deprecated?: boolean;
  /**
   * A verbose explanation of the operation behavior. {@link https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown GFM syntax} can be used for rich text representation.
   */
  description?: string;
  /**
   * Additional external documentation for this operation.
   */
  externalDocs?: ExternalDocumentationObject$2;
  /**
   * Unique string used to identify the operation. The id MUST be unique among all operations described in the API. Tools and libraries MAY use the operationId to uniquely identify an operation, therefore, it is recommended to follow common programming naming conventions.
   */
  operationId?: string;
  /**
   * A list of parameters that are applicable for this operation. If a parameter is already defined at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#pathItemParameters Path Item}, the new definition will override it, but can never remove it. The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterName name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn location}. The list can use the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#reference-object Reference Object} to link to parameters that are defined at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerParameters Swagger Object's parameters}. There can be one "body" parameter at most.
   */
  parameters?: Array<ParameterObject$2 | ReferenceObject$2>;
  /**
   * A list of MIME types the operation can produce. This overrides the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerProduces `produces`} definition at the Swagger Object. An empty value MAY be used to clear the global definition. Value MUST be as described under {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#mime-types Mime Types}.
   */
  produces?: Array<string>;
  /**
   * **Required**. The list of possible responses as they are returned from executing this operation.
   */
  responses: ResponsesObject$2;
  /**
   * The transfer protocol for the operation. Values MUST be from the list: `"http"`, `"https"`, `"ws"`, `"wss"`. The value overrides the Swagger Object {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerSchemes `schemes`} definition.
   */
  schemes?: Array<'http' | 'https' | 'ws' | 'wss'>;
  /**
   * A declaration of which security schemes are applied for this operation. The list of values describes alternative security schemes that can be used (that is, there is a logical OR between the security requirements). This definition overrides any declared top-level {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerSecurity `security`}. To remove a top-level security declaration, an empty array can be used.
   */
  security?: Array<SecurityRequirementObject$2>;
  /**
   * A short summary of what the operation does. For maximum readability in the swagger-ui, this field SHOULD be less than 120 characters.
   */
  summary?: string;
  /**
   * A list of tags for API documentation control. Tags can be used for logical grouping of operations by resources or any other qualifier.
   */
  tags?: Array<string>;
  /**
   * A list of code samples associated with an operation.
   */
  'x-codeSamples'?: Array<CodeSampleObject>;
}
/**
 * Describes a single operation parameter.
 *
 * A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterName name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn location}.
 *
 * There are five possible parameter types.
 *
 * - Path - Used together with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#path-templating Path Templating}, where the parameter value is actually part of the operation's URL. This does not include the host or base path of the API. For example, in `/items/{itemId}`, the path parameter is `itemId`.
 * - Query - Parameters that are appended to the URL. For example, in `/items?id=###`, the query parameter is `id`.
 * - Header - Custom headers that are expected as part of the request.
 * - Body - The payload that's appended to the HTTP request. Since there can only be one payload, there can only be _one_ body parameter. The name of the body parameter has no effect on the parameter itself and is used for documentation purposes only. Since Form parameters are also in the payload, body and form parameters cannot exist together for the same operation.
 * - Form - Used to describe the payload of an HTTP request when either `application/x-www-form-urlencoded`, `multipart/form-data` or both are used as the content type of the request (in Swagger's definition, the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#operationConsumes `consumes`} property of an operation). This is the only parameter type that can be used to send files, thus supporting the `file` type. Since form parameters are sent in the payload, they cannot be declared together with a body parameter for the same operation. Form parameters have a different format based on the content-type used (for further details, consult {@link http://www.w3.org/TR/html401/interact/forms.html#h-17.13.4 http://www.w3.org/TR/html401/interact/forms.html#h-17.13.4}):
 *   - `application/x-www-form-urlencoded` - Similar to the format of Query parameters but as a payload. For example, `foo=1&bar=swagger` - both `foo` and `bar` are form parameters. This is normally used for simple parameters that are being transferred.
 *   - `multipart/form-data` - each parameter takes a section in the payload with an internal header. For example, for the header `Content-Disposition: form-data; name="submit-name"` the name of the parameter is `submit-name`. This type of form parameters is more commonly used for file transfers.
 *
 * **Parameter Object Examples**
 *
 * Body Parameters
 *
 * A body parameter with a referenced schema definition (normally for a model definition):
 *
 * @example
 * ```yaml
 * name: user
 * in: body
 * description: user to add to the system
 * required: true
 * schema:
 *   $ref: '#/definitions/User'
 * ```
 *
 * A body parameter that is an array of string values:
 *
 * @example
 * ```yaml
 * name: user
 * in: body
 * description: user to add to the system
 * required: true
 * schema:
 *   type: array
 *   items:
 *     type: string
 * ```
 *
 * Other Parameters
 *
 * A header parameter with an array of 64 bit integer numbers:
 *
 * @example
 * ```yaml
 * name: token
 * in: header
 * description: token to be passed as a header
 * required: true
 * type: array
 * items:
 *   type: integer
 *   format: int64
 * collectionFormat: csv
 * ```
 *
 * A path parameter of a string value:
 *
 * @example
 * ```yaml
 * name: username
 * in: path
 * description: username to fetch
 * required: true
 * type: string
 * ```
 *
 * An optional query parameter of a string value, allowing multiple values by repeating the query parameter:
 *
 * @example
 * ```yaml
 * name: id
 * in: query
 * description: ID of the object to fetch
 * required: false
 * type: array
 * items:
 *   type: string
 * collectionFormat: multi
 * ```
 *
 * A form data with file type for a file upload:
 *
 * @example
 * ```yaml
 * name: avatar
 * in: formData
 * description: The avatar of the user
 * required: true
 * type: file
 * ```
 */
type ParameterObject$2 = EnumExtensions & OpenAPIV2NullableExtensions & {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * A brief description of the parameter. This could contain examples of use. {@link https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown GFM syntax} can be used for rich text representation.
   */
  description?: string;
  /**
   * **Required**. The name of the parameter. Parameter names are _case sensitive_.
   *
   * - If {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn `in`} is `"path"`, the `name` field MUST correspond to the associated path segment from the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#pathsPath path} field in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#paths-object Paths Object}. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#path-templating Path Templating} for further information.
   * - For all other cases, the `name` corresponds to the parameter name used based on the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn `in`} property.
   */
  name: string;
  /**
   * Determines whether this parameter is mandatory. If the parameter is {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn `in`} "path", this property is **required** and its value MUST be `true`. Otherwise, the property MAY be included and its default value is `false`.
   */
  required?: boolean;
} & ({
  /**
   * **Required**. The location of the parameter. Possible values are "query", "header", "path", "formData" or "body".
   */
  in: 'body';
  /**
   * **Required**. The schema defining the type used for the body parameter.
   */
  schema: SchemaObject$2;
} | {
  /**
   * Sets the ability to pass empty-valued parameters. This is valid only for either `query` or `formData` parameters and allows you to send a parameter with a name only or an empty value. Default value is `false`.
   */
  allowEmptyValue?: boolean;
  /**
   * Determines the format of the array if type array is used. Possible values are:
   *
   * - `csv` - comma separated values `foo,bar`.
   * - `ssv` - space separated values `foo bar`.
   * - `tsv` - tab separated values `foo\tbar`.
   * - `pipes` - pipe separated values `foo|bar`.
   * - `multi` - corresponds to multiple parameter instances instead of multiple values for a single instance `foo=bar&foo=baz`. This is valid only for parameters {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn `in`} "query" or "formData".
   *
   * Default value is `csv`.
   */
  collectionFormat?: 'csv' | 'multi' | 'pipes' | 'ssv' | 'tsv';
  /**
   * Declares the value of the parameter that the server will use if none is provided, for example a "count" to control the number of results per page might default to 100 if not supplied by the client in the request. (Note: "default" has no meaning for required parameters.) See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-6.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-6.2}. Unlike JSON Schema this value MUST conform to the defined {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterType `type`} for this parameter.
   */
  default?: unknown;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.5.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.5.1}.
   */
  enum?: Array<unknown>;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2}.
   */
  exclusiveMaximum?: boolean;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3}.
   */
  exclusiveMinimum?: boolean;
  /**
   * The extending format for the previously mentioned {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterType `type`}. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#dataTypeFormat Data Type Formats} for further details.
   */
  format?: string;
  /**
   * **Required**. The location of the parameter. Possible values are "query", "header", "path", "formData" or "body".
   */
  in: 'formData' | 'header' | 'path' | 'query';
  /**
   * **Required if {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterType `type`} is "array"**. Describes the type of items in the array.
   */
  items?: ItemsObject;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.2}.
   */
  maxItems?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.1}.
   */
  maxLength?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.2}.
   */
  maximum?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.3}.
   */
  minItems?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.2 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.2}.
   */
  minLength?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.3}.
   */
  minimum?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.1 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.1.1}.
   */
  multipleOf?: number;
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.3 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.2.3}.
   */
  pattern?: string;
  /**
   * **Required**. The type of the parameter. Since the parameter is not located at the request body, it is limited to simple types (that is, not an object). The value MUST be one of `"string"`, `"number"`, `"integer"`, `"boolean"`, `"array"` or `"file"`. If `type` is `"file"`, the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#operationConsumes `consumes`} MUST be either `"multipart/form-data"`, `"application/x-www-form-urlencoded"` or both and the parameter MUST be {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn `in`} `"formData"`.
   */
  type: 'array' | 'boolean' | 'file' | 'integer' | 'number' | 'string';
  /**
   * See {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.4 https://tools.ietf.org/html/draft-fge-json-schema-validation-00#section-5.3.4}.
   */
  uniqueItems?: boolean;
});
/**
 * An object to hold parameters to be reused across operations. Parameter definitions can be referenced to the ones defined here.
 *
 * This does _not_ define global operation parameters.
 *
 * **Parameters Definition Object Example**
 *
 * @example
 * ```yaml
 * skipParam:
 *   name: skip
 *   in: query
 *   description: number of items to skip
 *   required: true
 *   type: integer
 *   format: int32
 * limitParam:
 *   name: limit
 *   in: query
 *   description: max records to return
 *   required: true
 *   type: integer
 *   format: int32
 * ```
 */
interface ParametersDefinitionsObject {
  /**
   * A single parameter definition, mapping a "name" to the parameter it defines.
   */
  [name: string]: ParameterObject$2;
}
/**
 * Describes the operations available on a single path. A Path Item may be empty, due to {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#security-filtering ACL constraints}. The path itself is still exposed to the documentation viewer but they will not know which operations and parameters are available.
 *
 * @example
 * ```yaml
 * get:
 *   description: Returns pets based on ID
 *   summary: Find pets by ID
 *   operationId: getPetsById
 *   produces:
 *   - application/json
 *   - text/html
 *   responses:
 *     '200':
 *       description: pet response
 *       schema:
 *         type: array
 *         items:
 *           $ref: '#/definitions/Pet'
 *     default:
 *       description: error payload
 *       schema:
 *         $ref: '#/definitions/ErrorModel'
 * parameters:
 * - name: id
 *   in: path
 *   description: ID of pet to use
 *   required: true
 *   type: array
 *   items:
 *     type: string
 *   collectionFormat: csv
 * ```
 */
interface PathItemObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * Allows for an external definition of this path item. The referenced structure MUST be in the format of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#path-item-object Path Item Object}. If there are conflicts between the referenced definition and this Path Item's definition, the behavior is _undefined_.
   */
  $ref?: string;
  /**
   * A definition of a DELETE operation on this path.
   */
  delete?: OperationObject$2;
  /**
   * A definition of a GET operation on this path.
   */
  get?: OperationObject$2;
  /**
   * A definition of a HEAD operation on this path.
   */
  head?: OperationObject$2;
  /**
   * A definition of a OPTIONS operation on this path.
   */
  options?: OperationObject$2;
  /**
   * A list of parameters that are applicable for all the operations described under this path. These parameters can be overridden at the operation level, but cannot be removed there. The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterName name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#parameterIn location}. The list can use the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#reference-object Reference Object} to link to parameters that are defined at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerParameters Swagger Object's parameters}. There can be one "body" parameter at most.
   */
  parameters?: Array<ParameterObject$2 | ReferenceObject$2>;
  /**
   * A definition of a PATCH operation on this path.
   */
  patch?: OperationObject$2;
  /**
   * A definition of a POST operation on this path.
   */
  post?: OperationObject$2;
  /**
   * A definition of a PUT operation on this path.
   */
  put?: OperationObject$2;
}
/**
 * Holds the relative paths to the individual endpoints. The path is appended to the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerBasePath `basePath`} in order to construct the full URL. The Paths may be empty, due to {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#security-filtering ACL constraints}.
 *
 * @example
 * ```yaml
 * /pets:
 *   get:
 *     description: Returns all pets from the system that the user has access to
 *     produces:
 *     - application/json
 *     responses:
 *       '200':
 *         description: A list of pets.
 *         schema:
 *           type: array
 *           items:
 *             $ref: '#/definitions/pet'
 * ```
 */
interface PathsObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * A relative path to an individual endpoint. The field name MUST begin with a slash. The path is appended to the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerBasePath `basePath`} in order to construct the full URL. {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#path-templating Path templating} is allowed.
   */
  [path: `/${string}`]: PathItemObject$2;
}
/**
 * A simple object to allow referencing other definitions in the specification. It can be used to reference parameters and responses that are defined at the top level for reuse.
 *
 * The Reference Object is a {@link http://tools.ietf.org/html/draft-pbryan-zyp-json-ref-02 JSON Reference} that uses a {@link http://tools.ietf.org/html/rfc6901 JSON Pointer} as its value. For this specification, only {@link https://tools.ietf.org/html/draft-zyp-json-schema-04#section-7.2.3 canonical dereferencing} is supported.
 *
 * **Reference Object Example**
 *
 * @example
 * ```yaml
 * $ref: '#/definitions/Pet'
 * ```
 *
 * **Relative Schema File Example**
 *
 * @example
 * ```yaml
 * $ref: 'Pet.yaml'
 * ```
 *
 * **Relative Files With Embedded Schema Example**
 *
 * @example
 * ```yaml
 * $ref: 'definitions.yaml#/Pet'
 * ```
 */
interface ReferenceObject$2 {
  /**
   * **Required**. The reference string.
   */
  $ref: string;
}
/**
 * Describes a single response from an API Operation.
 *
 * **Response Object Examples**
 *
 * Response of an array of a complex type:
 *
 * @example
 * ```yaml
 * description: A complex object array response
 * schema:
 *   type: array
 *   items:
 *     $ref: '#/definitions/VeryComplexType'
 * ```
 *
 * Response with a string type:
 *
 * @example
 * ```yaml
 * description: A simple string response
 * schema:
 *   type: string
 * ```
 *
 * Response with headers:
 *
 * @example
 * ```yaml
 * description: A simple string response
 * schema:
 *   type: string
 * headers:
 *   X-Rate-Limit-Limit:
 *     description: The number of allowed requests in the current period
 *     type: integer
 *   X-Rate-Limit-Remaining:
 *     description: The number of remaining requests in the current period
 *     type: integer
 *   X-Rate-Limit-Reset:
 *     description: The number of seconds left in the current period
 *     type: integer
 * ```
 *
 * Response with no return value:
 *
 * @example
 * ```yaml
 * description: object created
 * ```
 */
interface ResponseObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * **Required**. A short description of the response. {@link https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown GFM syntax} can be used for rich text representation.
   */
  description: string;
  /**
   * An example of the response message.
   */
  examples?: ExampleObject$2;
  /**
   * A list of headers that are sent with the response.
   */
  headers?: HeadersObject;
  /**
   * A definition of the response structure. It can be a primitive, an array or an object. If this field does not exist, it means no content is returned as part of the response. As an extension to the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#schema-object Schema Object}, its root `type` value may also be `"file"`. This SHOULD be accompanied by a relevant `produces` mime-type.
   */
  schema?: SchemaObject$2;
}
/**
 * An object to hold responses to be reused across operations. Response definitions can be referenced to the ones defined here.
 *
 * This does _not_ define global operation responses.
 *
 * **Responses Definitions Object Example**
 *
 * @example
 * ```yaml
 * NotFound:
 *   description: Entity not found.
 * IllegalInput:
 *   description: Illegal input for operation.
 * GeneralError:
 *   description: General Error
 *   schema:
 *     $ref: '#/definitions/GeneralError'
 * ```
 */
interface ResponsesDefinitionsObject {
  /**
   * A single response definition, mapping a "name" to the response it defines.
   */
  [name: string]: ResponseObject$2;
}
/**
 * A container for the expected responses of an operation. The container maps a HTTP response code to the expected response. It is not expected from the documentation to necessarily cover all possible HTTP response codes, since they may not be known in advance. However, it is expected from the documentation to cover a successful operation response and any known errors.
 *
 * The `default` can be used as the default response object for all HTTP codes that are not covered individually by the specification.
 *
 * The `Responses Object` MUST contain at least one response code, and it SHOULD be the response for a successful operation call.
 *
 * **Responses Object Example**
 *
 * A 200 response for successful operation and a default response for others (implying an error):
 *
 * @example
 * ```yaml
 * '200':
 *   description: a pet to be returned
 *   schema:
 *     $ref: '#/definitions/Pet'
 * default:
 *   description: Unexpected error
 *   schema:
 *     $ref: '#/definitions/ErrorModel'
 * ```
 */
interface ResponsesObject$2 {
  /**
   * Any {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#http-status-codes HTTP status code} can be used as the property name (one property per HTTP status code). Describes the expected response for that HTTP status code. {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#reference-object Reference Object} can be used to link to a response that is defined at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerResponses Swagger Object's responses} section.
   */
  [httpStatusCode: string]: ResponseObject$2 | ReferenceObject$2 | undefined;
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: any;
  /**
   * The documentation of responses other than the ones declared for specific HTTP response codes. It can be used to cover undeclared responses. {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#reference-object Reference Object} can be used to link to a response that is defined at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#swaggerResponses Swagger Object's responses} section.
   */
  default?: ResponseObject$2 | ReferenceObject$2;
}
/**
 * The Schema Object allows the definition of input and output data types. These types can be objects, but also primitives and arrays. This object is based on the {@link http://json-schema.org/ JSON Schema Specification Draft 4} and uses a predefined subset of it. On top of this subset, there are extensions provided by this specification to allow for more complete documentation.
 *
 * Further information about the properties can be found in {@link https://tools.ietf.org/html/draft-zyp-json-schema-04 JSON Schema Core} and {@link https://tools.ietf.org/html/draft-fge-json-schema-validation-00 JSON Schema Validation}. Unless stated otherwise, the property definitions follow the JSON Schema specification as referenced here.
 *
 * The following properties are taken directly from the JSON Schema definition and follow the same specifications:
 *
 * - $ref - As a {@link https://tools.ietf.org/html/draft-pbryan-zyp-json-ref-03 JSON Reference}
 * - format (See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#dataTypeFormat Data Type Formats} for further details)
 * - title
 * - description ({@link https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown GFM syntax} can be used for rich text representation)
 * - default (Unlike JSON Schema, the value MUST conform to the defined type for the Schema Object)
 * - multipleOf
 * - maximum
 * - exclusiveMaximum
 * - minimum
 * - exclusiveMinimum
 * - maxLength
 * - minLength
 * - pattern
 * - maxItems
 * - minItems
 * - uniqueItems
 * - maxProperties
 * - minProperties
 * - required
 * - enum
 * - type
 *
 * The following properties are taken from the JSON Schema definition but their definitions were adjusted to the Swagger Specification. Their definition is the same as the one from JSON Schema, only where the original definition references the JSON Schema definition, the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#schema-object Schema Object} definition is used instead.
 *
 * - items
 * - allOf
 * - properties
 * - additionalProperties
 *
 * Other than the JSON Schema subset fields, the following fields may be used for further schema documentation.
 *
 * **Composition and Inheritance (Polymorphism)**
 *
 * Swagger allows combining and extending model definitions using the `allOf` property of JSON Schema, in effect offering model composition. `allOf` takes in an array of object definitions that are validated _independently_ but together compose a single object.
 *
 * While composition offers model extensibility, it does not imply a hierarchy between the models. To support polymorphism, Swagger adds the support of the `discriminator` field. When used, the `discriminator` will be the name of the property used to decide which schema definition is used to validate the structure of the model. As such, the `discriminator` field MUST be a required field. The value of the chosen property has to be the friendly name given to the model under the `definitions` property. As such, inline schema definitions, which do not have a given id, _cannot_ be used in polymorphism.
 *
 * **XML Modeling**
 *
 * The {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#schemaXml xml} property allows extra definitions when translating the JSON definition to XML. The {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#xml-object XML Object} contains additional information about the available options.
 *
 * **Schema Object Examples**
 *
 * Primitive Sample
 *
 * Unlike previous versions of Swagger, Schema definitions can be used to describe primitive and arrays as well.
 *
 * @example
 * ```yaml
 * type: string
 * format: email
 * ```
 *
 * Simple Model
 *
 * @example
 * ```yaml
 * type: object
 * required:
 * - name
 * properties:
 *   name:
 *     type: string
 *   address:
 *     $ref: '#/definitions/Address'
 *   age:
 *     type: integer
 *     format: int32
 *     minimum: 0
 * ```
 *
 * Model with Map/Dictionary Properties
 *
 * For a simple string to string mapping:
 *
 * @example
 * ```yaml
 * type: object
 * additionalProperties:
 *   type: string
 * ```
 *
 * For a string to model mapping:
 *
 * @example
 * ```yaml
 * type: object
 * additionalProperties:
 *   $ref: '#/definitions/ComplexModel'
 * ```
 *
 * Model with Example
 *
 * @example
 * ```yaml
 * type: object
 * properties:
 *   id:
 *     type: integer
 *     format: int64
 *   name:
 *     type: string
 * required:
 * - name
 * example:
 *   name: Puma
 *   id: 1
 * ```
 *
 * Models with Composition
 *
 * @example
 * ```yaml
 * definitions:
 *   ErrorModel:
 *     type: object
 *     required:
 *     - message
 *     - code
 *     properties:
 *       message:
 *         type: string
 *       code:
 *         type: integer
 *         minimum: 100
 *         maximum: 600
 *   ExtendedErrorModel:
 *     allOf:
 *     - $ref: '#/definitions/ErrorModel'
 *     - type: object
 *       required:
 *       - rootCause
 *       properties:
 *         rootCause:
 *           type: string
 * ```
 *
 * Models with Polymorphism Support
 *
 * @example
 * ```yaml
 * definitions:
 *   Pet:
 *     type: object
 *     discriminator: petType
 *     properties:
 *       name:
 *         type: string
 *       petType:
 *         type: string
 *     required:
 *     - name
 *     - petType
 *   Cat:
 *     description: A representation of a cat
 *     allOf:
 *     - $ref: '#/definitions/Pet'
 *     - type: object
 *       properties:
 *         huntingSkill:
 *           type: string
 *           description: The measured skill for hunting
 *           default: lazy
 *           enum:
 *           - clueless
 *           - lazy
 *           - adventurous
 *           - aggressive
 *       required:
 *       - huntingSkill
 *   Dog:
 *     description: A representation of a dog
 *     allOf:
 *     - $ref: '#/definitions/Pet'
 *     - type: object
 *       properties:
 *         packSize:
 *           type: integer
 *           format: int32
 *           description: the size of the pack the dog is from
 *           default: 0
 *           minimum: 0
 *       required:
 *       - packSize
 * ```
 */
interface SchemaObject$2 extends Document$6, EnumExtensions, OpenAPIV2NullableExtensions {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * The `additionalProperties` keyword is used to control the handling of extra stuff, that is, properties whose names are not listed in the `properties` keyword or match any of the regular expressions in the `patternProperties` keyword. By default any additional properties are allowed.
   *
   * The value of the `additionalProperties` keyword is a schema that will be used to validate any properties in the {@link https://json-schema.org/learn/glossary#instance instance} that are not matched by `properties` or `patternProperties`. Setting the `additionalProperties` schema to `false` means no additional properties will be allowed.
   *
   * It's important to note that `additionalProperties` only recognizes properties declared in the same {@link https://json-schema.org/learn/glossary#subschema subschema} as itself. So, `additionalProperties` can restrict you from "extending" a schema using {@link https://json-schema.org/understanding-json-schema/reference/combining combining} keywords such as {@link https://json-schema.org/understanding-json-schema/reference/combining#allof allOf}.
   */
  additionalProperties?: SchemaObject$2 | boolean;
  /**
   * `allOf`: (AND) Must be valid against _all_ of the {@link https://json-schema.org/learn/glossary#subschema subschemas}
   *
   * To validate against `allOf`, the given data must be valid against all of the given subschemas.
   *
   * {@link https://json-schema.org/understanding-json-schema/reference/combining#allof allOf} can not be used to "extend" a schema to add more details to it in the sense of object-oriented inheritance. {@link https://json-schema.org/learn/glossary#instance Instances} must independently be valid against "all of" the schemas in the `allOf`. See the section on {@link https://json-schema.org/understanding-json-schema/reference/object#extending Extending Closed Schemas} for more information.
   */
  allOf?: Array<SchemaObject$2>;
  /**
   * Adds support for polymorphism. The discriminator is the schema property name that is used to differentiate between other schema that inherit this schema. The property name used MUST be defined at this schema and it MUST be in the `required` property list. When used, the value MUST be the name of this schema or any schema that inherits it.
   */
  discriminator?: string;
  /**
   * A free-form property to include an example of an instance for this schema.
   */
  example?: unknown;
  /**
   * Additional external documentation for this schema.
   */
  externalDocs?: ExternalDocumentationObject$2;
  /**
   * List validation is useful for arrays of arbitrary length where each item matches the same schema. For this kind of array, set the `items` {@link https://json-schema.org/learn/glossary#keyword keyword} to a single schema that will be used to validate all of the items in the array.
   *
   * The `items` keyword can be used to control whether it's valid to have additional items in a tuple beyond what is defined in `prefixItems`. The value of the `items` keyword is a schema that all additional items must pass in order for the keyword to validate.
   *
   * Note that `items` doesn't "see inside" any {@link https://json-schema.org/learn/glossary#instance instances} of `allOf`, `anyOf`, or `oneOf` in the same {@link https://json-schema.org/learn/glossary#subschema subschema}.
   */
  items?: SchemaObject$2;
  /**
   * The properties (key-value pairs) on an object are defined using the `properties` {@link https://json-schema.org/learn/glossary#keyword keyword}. The value of `properties` is an object, where each key is the name of a property and each value is a {@link https://json-schema.org/learn/glossary#schema schema} used to validate that property. Any property that doesn't match any of the property names in the `properties` keyword is ignored by this keyword.
   */
  properties?: Record<string, SchemaObject$2>;
  /**
   * Relevant only for Schema `"properties"` definitions. Declares the property as "read only". This means that it MAY be sent as part of a response but MUST NOT be sent as part of the request. Properties marked as `readOnly` being `true` SHOULD NOT be in the `required` list of the defined schema. Default value is `false`.
   */
  readOnly?: boolean;
  /**
   * This MAY be used only on properties schemas. It has no effect on root schemas. Adds Additional metadata to describe the XML representation format of this property.
   */
  xml?: XMLObject$2;
}
/**
 * Lists the available scopes for an OAuth2 security scheme.
 *
 * **Scopes Object Example**
 *
 * @example
 * ```yaml
 * write:pets: modify pets in your account
 * read:pets: read your pets
 * ```
 */
interface ScopesObject {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: any;
  /**
   * Maps between a name of a scope to a short description of it (as the value of the property).
   */
  [name: string]: string;
}
/**
 * A declaration of the security schemes available to be used in the specification. This does not enforce the security schemes on the operations and only serves to provide the relevant details for each scheme.
 *
 * **Security Definitions Object Example**
 *
 * @example
 * ```yaml
 * api_key:
 *   type: apiKey
 *   name: api_key
 *   in: header
 * petstore_auth:
 *   type: oauth2
 *   authorizationUrl: http://swagger.io/api/oauth/dialog
 *   flow: implicit
 *   scopes:
 *     write:pets: modify pets in your account
 *     read:pets: read your pets
 * ```
 */
interface SecurityDefinitionsObject {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: any;
  /**
   * A single security scheme definition, mapping a "name" to the scheme it defines.
   */
  [name: string]: SecuritySchemeObject$2;
}
/**
 * Lists the required security schemes to execute this operation. The object can have multiple security schemes declared in it which are all required (that is, there is a logical AND between the schemes).
 *
 * The name used for each property MUST correspond to a security scheme declared in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#security-definitions-object Security Definitions}.
 *
 * **Security Requirement Object Examples**
 *
 * Non-OAuth2 Security Requirement
 *
 * @example
 * ```yaml
 * api_key: []
 * ```
 *
 * OAuth2 Security Requirement
 *
 * @example
 * ```yaml
 * petstore_auth:
 * - write:pets
 * - read:pets
 * ```
 */
interface SecurityRequirementObject$2 {
  /**
   * Each name must correspond to a security scheme which is declared in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#securityDefinitions Security Definitions}. If the security scheme is of type `"oauth2"`, then the value is a list of scope names required for the execution. For other security scheme types, the array MUST be empty.
   */
  [name: string]: Array<string>;
}
/**
 * Allows the definition of a security scheme that can be used by the operations. Supported schemes are basic authentication, an API key (either as a header or as a query parameter) and OAuth2's common flows (implicit, password, application and access code).
 *
 * **Security Scheme Object Example**
 *
 * Basic Authentication Sample
 *
 * @example
 * ```yaml
 * type: basic
 * ```
 *
 * API Key Sample
 *
 * @example
 * ```yaml
 * type: apiKey
 * name: api_key
 * in: header
 * ```
 *
 * Implicit OAuth2 Sample
 *
 * @example
 * ```yaml
 * type: oauth2
 * authorizationUrl: http://swagger.io/api/oauth/dialog
 * flow: implicit
 * scopes:
 *   write:pets: modify pets in your account
 *   read:pets: read your pets
 * ```
 */
type SecuritySchemeObject$2 = {
  /**
   * A short description for security scheme.
   */
  description?: string;
} & ({
  /**
   * **Required** The location of the API key. Valid values are `"query"` or `"header"`.
   */
  in: 'header' | 'query';
  /**
   * **Required**. The name of the header or query parameter to be used.
   */
  name: string;
  /**
   * **Required**. The type of the security scheme. Valid values are `"basic"`, `"apiKey"` or `"oauth2"`.
   */
  type: 'apiKey';
} | {
  /**
   * **Required (`"implicit"`, `"accessCode"`)**. The authorization URL to be used for this flow. This SHOULD be in the form of a URL.
   */
  authorizationUrl?: string;
  /**
   * **Required**. The flow used by the OAuth2 security scheme. Valid values are `"implicit"`, `"password"`, `"application"` or `"accessCode"`.
   */
  flow: 'accessCode' | 'application' | 'implicit' | 'password';
  /**
   * **Required**. The available scopes for the OAuth2 security scheme.
   */
  scopes: ScopesObject;
  /**
   * **Required (`"password"`, `"application"`, `"accessCode"`)**. The token URL to be used for this flow. This SHOULD be in the form of a URL.
   */
  tokenUrl?: string;
  /**
   * **Required**. The type of the security scheme. Valid values are `"basic"`, `"apiKey"` or `"oauth2"`.
   */
  type: 'oauth2';
} | {
  /**
   * **Required**. The type of the security scheme. Valid values are `"basic"`, `"apiKey"` or `"oauth2"`.
   */
  type: 'basic';
});
/**
 * Allows adding meta data to a single tag that is used by the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#operation-object Operation Object}. It is not mandatory to have a Tag Object per tag used there.
 *
 * **Tag Object Example**
 *
 * @example
 * ```yaml
 * name: pet
 * description: Pets operations
 * ```
 */
interface TagObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * A short description for the tag. {@link https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown GFM syntax} can be used for rich text representation.
   */
  description?: string;
  /**
   * Additional external documentation for this tag.
   */
  externalDocs?: ExternalDocumentationObject$2;
  /**
   * **Required**. The name of the tag.
   */
  name: string;
}
/**
 * A metadata object that allows for more fine-tuned XML model definitions.
 *
 * When using arrays, XML element names are _not_ inferred (for singular/plural forms) and the `name` property should be used to add that information. See examples for expected behavior.
 */
interface XMLObject$2 {
  /**
   * Allows extensions to the Swagger Schema. The field name MUST begin with `x-`, for example, `x-internal-id`. The value can be `null`, a primitive, an array or an object. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#specification-extensions Vendor Extensions} for further details.
   */
  [name: `x-${string}`]: unknown;
  /**
   * Declares whether the property definition translates to an attribute instead of an element. Default value is `false`.
   */
  attribute?: boolean;
  /**
   * Replaces the name of the element/attribute used for the described schema property. When defined within the Items Object (`items`), it will affect the name of the individual XML elements within the list. When defined alongside `type` being `array` (outside the `items`), it will affect the wrapping element and only if `wrapped` is `true`. If `wrapped` is `false`, it will be ignored.
   */
  name?: string;
  /**
   * The URL of the namespace definition. Value SHOULD be in the form of a URL.
   */
  namespace?: string;
  /**
   * The prefix to be used for the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md#xmlName name}.
   */
  prefix?: string;
  /**
   * MAY be used only for an array definition. Signifies whether the array is wrapped (for example, `<books><book/><book/></books>`) or unwrapped (`<book/><book/>`). Default value is `false`. The definition takes effect only when defined alongside `type` being `array` (outside the `items`).
   */
  wrapped?: boolean;
}
declare namespace index_d_exports$2 {
  export { ContactObject$2 as ContactObject, DefinitionsObject, Document$3 as Document, ExampleObject$2 as ExampleObject, ExternalDocumentationObject$2 as ExternalDocumentationObject, HeaderObject$2 as HeaderObject, HeadersObject, InfoObject$2 as InfoObject, ItemsObject, LicenseObject$2 as LicenseObject, OpenAPIV2NullableExtensions as NullableExtensions, OperationObject$2 as OperationObject, ParameterObject$2 as ParameterObject, ParametersDefinitionsObject, PathItemObject$2 as PathItemObject, PathsObject$2 as PathsObject, ReferenceObject$2 as ReferenceObject, ResponseObject$2 as ResponseObject, ResponsesDefinitionsObject, ResponsesObject$2 as ResponsesObject, SchemaObject$2 as SchemaObject, ScopesObject, SecurityDefinitionsObject, SecurityRequirementObject$2 as SecurityRequirementObject, SecuritySchemeObject$2 as SecuritySchemeObject, TagObject$2 as TagObject, XMLObject$2 as XMLObject };
}
//#endregion
//#region src/openapi/v3/spec.d.ts
/**
 * This is the root object of the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#openapi-description OpenAPI Description}.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 */
interface Document$2 extends SpecExtensions {
  /**
   * An element to hold various Objects for the OpenAPI Description.
   */
  components?: ComponentsObject$1;
  /**
   * Additional external documentation.
   */
  externalDocs?: ExternalDocumentationObject$1;
  /**
   * **REQUIRED**. Provides metadata about the API. The metadata MAY be used by tooling as required.
   */
  info: InfoObject$1;
  /**
   * **REQUIRED**. This string MUST be the {@link https://semver.org/spec/v2.0.0.html semantic version number} of the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.3.md#versions OpenAPI Specification version} that the OpenAPI document uses. The `openapi` field SHOULD be used by tooling specifications and clients to interpret the OpenAPI document. This is _not_ related to the API {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.3.md#infoVersion `info.version`} string.
   */
  openapi: '3.0.0' | '3.0.1' | '3.0.2' | '3.0.3' | '3.0.4';
  /**
   * **REQUIRED**. The available paths and operations for the API.
   */
  paths: PathsObject$1;
  /**
   * A declaration of which security mechanisms can be used across the API. The list of values includes alternative Security Requirement Objects that can be used. Only one of the Security Requirement Objects need to be satisfied to authorize a request. Individual operations can override this definition. The list can be incomplete, up to being empty or absent. To make security explicitly optional, an empty security requirement (`{}`) can be included in the array.
   */
  security?: Array<SecurityRequirementObject$1>;
  /**
   * An array of Server Objects, which provide connectivity information to a target server. If the `servers` field is not provided, or is an empty array, the default value would be a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#server-object Server Object} with a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#server-url url} value of `/`.
   */
  servers?: Array<ServerObject$1>;
  /**
   * A list of tags used by the OpenAPI Description with additional metadata. The order of the tags can be used to reflect on their order by the parsing tools. Not all tags that are used by the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#operation-object Operation Object} must be declared. The tags that are not declared MAY be organized randomly or based on the tools' logic. Each tag name in the list MUST be unique.
   */
  tags?: Array<TagObject$1>;
}
/**
 * A map of possible out-of band callbacks related to the parent operation. Each value in the map is a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#path-item-object Path Item Object} that describes a set of requests that may be initiated by the API provider and the expected responses. The key value used to identify the Path Item Object is an expression, evaluated at runtime, that identifies a URL to use for the callback operation.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface CallbackObject$1 extends SpecExtensions {
  /**
   * A Path Item Object used to define a callback request and expected responses. A {@link https://learn.openapis.org/examples/v3.0/callback-example.html complete example} is available.
   */
  [expression: string]: PathItemObject$1 | ReferenceObject$1 | unknown;
}
/**
 * Holds a set of reusable objects for different aspects of the OAS. All objects defined within the Components Object will have no effect on the API unless they are explicitly referenced from outside the Components Object.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * All the fixed fields declared above are objects that MUST use keys that match the regular expression: `^[a-zA-Z0-9\.\-_]+$`.
 *
 * TODO: examples
 */
interface ComponentsObject$1 extends SpecExtensions {
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#callback-object Callback Objects}.
   */
  callbacks?: Record<string, CallbackObject$1 | ReferenceObject$1>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#example-object Example Objects}.
   */
  examples?: Record<string, ExampleObject$1 | ReferenceObject$1>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#header-object Header Objects}.
   */
  headers?: Record<string, HeaderObject$1 | ReferenceObject$1>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#link-object Link Objects}.
   */
  linkes?: Record<string, LinkObject$1 | ReferenceObject$1>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-object Parameter Objects}.
   */
  parameters?: Record<string, ParameterObject$1 | ReferenceObject$1>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#request-body-object Request Body Objects}.
   */
  requestBodies?: Record<string, RequestBodyObject$1 | ReferenceObject$1>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#response-object Response Objects}.
   */
  responses?: Record<string, ResponseObject$1 | ReferenceObject$1>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Objects}.
   */
  schemas?: Record<string, SchemaObject$1 | ReferenceObject$1>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#security-scheme-object Security Scheme Objects}.
   */
  securitySchemes?: Record<string, SecuritySchemeObject$1 | ReferenceObject$1>;
}
/**
 * Contact information for the exposed API.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * ```yaml
 * name: API Support
 * url: https://www.example.com/support
 * email: support@example.com
 * ```
 */
interface ContactObject$1 extends SpecExtensions {
  /**
   * The email address of the contact person/organization. This MUST be in the form of an email address.
   */
  email?: string;
  /**
   * The identifying name of the contact person/organization.
   */
  name?: string;
  /**
   * The URL for the contact information. This MUST be in the form of a URL.
   */
  url?: string;
}
/**
 * When request bodies or response payloads may be one of a number of different schemas, a Discriminator Object gives a hint about the expected schema of the document. This hint can be used to aid in serialization, deserialization, and validation. The Discriminator Object does this by implicitly or explicitly associating the possible values of a named property with alternative schemas.
 *
 * Note that `discriminator` MUST NOT change the validation outcome of the schema.
 *
 * **Conditions for Using the Discriminator Object**
 *
 * TODO: content, examples
 */
interface DiscriminatorObject$1 {
  /**
   * An object to hold mappings between payload values and schema names or URI references.
   */
  mapping?: Record<string, string>;
  /**
   * **REQUIRED**. The name of the property in the payload that will hold the discriminating value. This property SHOULD be required in the payload schema, as the behavior when the property is absent is undefined.
   */
  propertyName: string;
}
/**
 * A single encoding definition applied to a single schema property. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-b-data-type-conversion Appendix B} for a discussion of converting values of various types to string representations.
 *
 * Properties are correlated with `multipart` parts using the {@link https://www.rfc-editor.org/rfc/rfc7578#section-4.2 `name` parameter} of `Content-Disposition: form-data`, and with `application/x-www-form-urlencoded` using the query string parameter names. In both cases, their order is implementation-defined.
 *
 * See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-e-percent-encoding-and-form-media-types Appendix E} for a detailed examination of percent-encoding concerns for form media types.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: default values examples
 * TODO: examples
 */
interface EncodingObject$1 extends SpecExtensions {
  /**
   * When this is true, parameter values are serialized using reserved expansion, as defined by {@link https://datatracker.ietf.org/doc/html/rfc6570#section-3.2.3 RFC6570}, which allows {@link https://datatracker.ietf.org/doc/html/rfc3986#section-2.2 RFC3986's reserved character set}, as well as percent-encoded triples, to pass through unchanged, while still percent-encoding all other disallowed characters (including `%` outside of percent-encoded triples). Applications are still responsible for percent-encoding reserved characters that are {@link https://datatracker.ietf.org/doc/html/rfc3986#section-3.4 not allowed in the query string} (`[`, `]`, `#`), or have a special meaning in `application/x-www-form-urlencoded` (`-`, `&`, `+`); see Appendices {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-c-using-rfc6570-based-serialization C} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-e-percent-encoding-and-form-media-types E} for details. The default value is `false`. This field SHALL be ignored if the request body media type is not `application/x-www-form-urlencoded`.
   */
  allowReserved?: boolean;
  /**
   * The `Content-Type` for encoding a specific property. The value is a comma-separated list, each element of which is either a specific media type (e.g. `image/png`) or a wildcard media type (e.g. `image/*`). Default value depends on the property type as shown in the table below.
   */
  contentType?: string;
  /**
   * When this is true, property values of type `array` or `object` generate separate parameters for each value of the array, or key-value-pair of the map. For other types of properties this field has no effect. When {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#encoding-style `style`} is `"form"`, the default value is `true`. For all other styles, the default value is `false`. Note that despite `false` being the default for `deepObject`, the combination of `false` with `deepObject` is undefined. This field SHALL be ignored if the request body media type is not `application/x-www-form-urlencoded`.
   */
  explode?: boolean;
  /**
   * A map allowing additional information to be provided as headers. `Content-Type` is described separately and SHALL be ignored in this section. This field SHALL be ignored if the request body media type is not a `multipart`.
   */
  headers?: Record<string, HeaderObject$1 | ReferenceObject$1>;
  /**
   * Describes how a specific property value will be serialized depending on its type. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-object Parameter Object} for details on the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-style `style`} field. The behavior follows the same values as `query` parameters, including default values. Note that the initial `?` used in query strings is not used in `application/x-www-form-urlencoded` message bodies, and MUST be removed (if using an RFC6570 implementation) or simply not added (if constructing the string manually). This field SHALL be ignored if the request body media type is not `application/x-www-form-urlencoded`.
   */
  style?: 'deepObject' | 'form' | 'label' | 'matrix' | 'pipeDelimited' | 'simple' | 'spaceDelimited';
}
/**
 * An object grouping an internal or external example value with basic `summary` and `description` metadata. This object is typically used in fields named `examples` (plural), and is a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#reference-object referenceable} alternative to older `example` (singular) fields that do not support referencing or metadata.
 *
 * Examples allow demonstration of the usage of properties, parameters and objects within OpenAPI.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * In all cases, the example value SHOULD be compatible with the schema of its associated value. Tooling implementations MAY choose to validate compatibility automatically, and reject the example value(s) if incompatible.
 *
 * TODO: examples
 */
interface ExampleObject$1 extends SpecExtensions {
  /**
   * Long description for the example. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * A URL that points to the literal example. This provides the capability to reference examples that cannot easily be included in JSON or YAML documents. The `value` field and `externalValue` field are mutually exclusive. See the rules for resolving {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#relative-references-in-urls Relative References}.
   */
  externalValue?: string;
  /**
   * Short description for the example.
   */
  summary?: string;
  /**
   * Embedded literal example. The `value` field and `externalValue` field are mutually exclusive. To represent examples of media types that cannot naturally represented in JSON or YAML, use a string value to contain the example, escaping where necessary.
   */
  value?: unknown;
}
/**
 * Allows referencing an external resource for extended documentation.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * **External Documentation Object Example**
 *
 * ```yaml
 * description: Find more info here
 * url: https://example.com
 * ```
 */
interface ExternalDocumentationObject$1 extends SpecExtensions {
  /**
   * A description of the target documentation. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * **REQUIRED**. The URL for the target documentation. This MUST be in the form of a URL.
   */
  url: string;
}
/**
 * Describes a single header for {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#response-headers HTTP responses} and for {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#encoding-headers individual parts in `multipart` representations}; see the relevant {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#response-object Response Object} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#encoding-object Encoding Object} documentation for restrictions on which headers can be described.
 *
 * The Header Object follows the structure of the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-object Parameter Object}, including determining its serialization strategy based on whether `schema` or `content` is present, with the following changes:
 * 1. `name` MUST NOT be specified, it is given in the corresponding `headers` map.
 * 1. `in` MUST NOT be specified, it is implicitly in `header`.
 * 1. All traits that are affected by the location MUST be applicable to a location of `header` (for example, {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-style `style`}). This means that `allowEmptyValue` and `allowReserved` MUST NOT be used, and `style`, if used, MUST be limited to `"simple"`.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
type HeaderObject$1 = Omit<ParameterObject$1, 'in' | 'name'>;
/**
 * The object provides metadata about the API. The metadata MAY be used by the clients if needed, and MAY be presented in editing or documentation generation tools for convenience.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * ```yaml
 * title: Example Pet Store App
 * description: This is an example server for a pet store.
 * termsOfService: https://example.com/terms/
 * contact:
 *   name: API Support
 *   url: https://www.example.com/support
 *   email: support@example.com
 * license:
 *   name: Apache 2.0
 *   url: https://www.apache.org/licenses/LICENSE-2.0.html
 * version: 1.0.1
 * ```
 */
interface InfoObject$1 extends SpecExtensions {
  /**
   * The contact information for the exposed API.
   */
  contact?: ContactObject$1;
  /**
   * A description of the API. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * The license information for the exposed API.
   */
  license?: LicenseObject$1;
  /**
   * A URL for the Terms of Service for the API. This MUST be in the form of a URL.
   */
  termsOfService?: string;
  /**
   * **REQUIRED**. The title of the API.
   */
  title: string;
  /**
   * **REQUIRED**. The version of the OpenAPI Document (which is distinct from the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#oas-version OpenAPI Specification version} or the version of the API being described or the version of the OpenAPI Description).
   */
  version: string;
}
/**
 * License information for the exposed API.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * ```yaml
 * name: Apache 2.0
 * url: https://www.apache.org/licenses/LICENSE-2.0.html
 * ```
 */
interface LicenseObject$1 extends SpecExtensions {
  /**
   * **REQUIRED**. The license name used for the API.
   */
  name: string;
  /**
   * A URL for the license used for the API. This MUST be in the form of a URL.
   */
  url?: string;
}
/**
 * The Link Object represents a possible design-time link for a response. The presence of a link does not guarantee the caller's ability to successfully invoke it, rather it provides a known relationship and traversal mechanism between responses and other operations.
 *
 * Unlike _dynamic links_ (i.e. links provided in the response payload), the OAS linking mechanism does not require link information in the runtime response.
 *
 * For computing links and providing instructions to execute them, a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#runtime-expressions runtime expression} is used for accessing values in an operation and using them as parameters while invoking the linked operation.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * A linked operation MUST be identified using either an `operationRef` or `operationId`. The identified or reference operation MUST be unique, and in the case of an `operationId`, it MUST be resolved within the scope of the OpenAPI Description (OAD). Because of the potential for name clashes, the `operationRef` syntax is preferred for multi-document OADs. However, because use of an operation depends on its URL path template in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#paths-object Paths Object}, operations from any {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#path-item-object Path Item Object} that is referenced multiple times within the OAD cannot be resolved unambiguously. In such ambiguous cases, the resulting behavior is implementation-defined and MAY result in an error.
 *
 * Note that it is not possible to provide a constant value to `parameters` that matches the syntax of a runtime expression. It is possible to have ambiguous parameter names, e.g. `name: "id"`, `in: "path"` and `name: "path.id", in: "query"`; this is NOT RECOMMENDED and the behavior is implementation-defined, however implementations SHOULD prefer the qualified interpretation (`path.id` as a path parameter), as the names can always be qualified to disambiguate them (e.g. using `query.path.id` for the query parameter).
 *
 * TODO: examples
 */
interface LinkObject$1 extends SpecExtensions {
  /**
   * A description of the link. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * The name of an _existing_, resolvable OAS operation, as defined with a unique `operationId`. This field is mutually exclusive of the `operationRef` field.
   */
  operationId?: string;
  /**
   * A URI reference to an OAS operation. This field is mutually exclusive of the `operationId` field, and MUST point to an {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#operation-object Operation Object}.
   */
  operationRef?: string;
  /**
   * A map representing parameters to pass to an operation as specified with `operationId` or identified via `operationRef`. The key is the parameter name to be used (optionally qualified with the parameter location, e.g. `path.id` for an `id` parameter in the path), whereas the value can be a constant or an expression to be evaluated and passed to the linked operation.
   */
  parameters?: Record<string, unknown | string>;
  /**
   * A literal value or {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#runtime-expressions {expression}} to use as a request body when calling the target operation.
   */
  requestBody?: unknown | string;
  /**
   * A server object to be used by the target operation.
   */
  server?: ServerObject$1;
}
/**
 * Each Media Type Object provides schema and examples for the media type identified by its key.
 *
 * When `example` or `examples` are provided, the example SHOULD match the specified schema and be in the correct format as specified by the media type and its encoding. The `example` and `examples` fields are mutually exclusive, and if either is present it SHALL _override_ any `example` in the schema. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#working-with-examples Working With Examples} for further guidance regarding the different ways of specifying examples, including non-JSON/YAML values.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface MediaTypeObject$1 extends SpecExtensions {
  /**
   * A map between a property name and its encoding information. The key, being the property name, MUST exist in the schema as a property. The `encoding` field SHALL only apply to {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#request-body-object Request Body Objects}, and only when the media type is `multipart` or `application/x-www-form-urlencoded`. If no Encoding Object is provided for a property, the behavior is determined by the default values documented for the Encoding Object.
   */
  encoding?: Record<string, EncodingObject$1>;
  /**
   * Example of the media type; see {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#working-with-examples Working With Examples}.
   */
  example?: unknown;
  /**
   * Examples of the media type; see {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#working-with-examples Working With Examples}.
   */
  examples?: Record<string, ExampleObject$1 | ReferenceObject$1>;
  /**
   * The schema defining the content of the request, response, parameter, or header.
   */
  schema?: SchemaObject$1 | ReferenceObject$1;
}
/**
 * Configuration details for a supported OAuth Flow
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface OAuthFlowObject$1 extends SpecExtensions {
  /**
   * **REQUIRED (`"implicit"`, `"authorizationCode"`)**. The authorization URL to be used for this flow. This MUST be in the form of a URL. The OAuth2 standard requires the use of TLS.
   */
  authorizationUrl?: string;
  /**
   * The URL to be used for obtaining refresh tokens. This MUST be in the form of a URL. The OAuth2 standard requires the use of TLS.
   */
  refreshUrl?: string;
  /**
   * **REQUIRED**. The available scopes for the OAuth2 security scheme. A map between the scope name and a short description for it. The map MAY be empty.
   */
  scopes: Record<string, string>;
  /**
   * **REQUIRED (`"password"`, `"clientCredentials"`, `"authorizationCode"`)**. The token URL to be used for this flow. This MUST be in the form of a URL. The OAuth2 standard requires the use of TLS.
   */
  tokenUrl?: string;
}
/**
 * Allows configuration of the supported OAuth Flows.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 */
interface OAuthFlowsObject$1 extends SpecExtensions {
  /**
   * Configuration for the OAuth Authorization Code flow. Previously called `accessCode` in OpenAPI 2.0.
   */
  authorizationCode?: OAuthFlowObject$1;
  /**
   * Configuration for the OAuth Client Credentials flow. Previously called `application` in OpenAPI 2.0.
   */
  clientCredentials?: OAuthFlowObject$1;
  /**
   * Configuration for the OAuth Implicit flow
   */
  implicit?: OAuthFlowObject$1;
  /**
   * Configuration for the OAuth Resource Owner Password flow
   */
  password?: OAuthFlowObject$1;
}
/**
 * Describes a single API operation on a path.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface OperationObject$1 extends SpecExtensions {
  /**
   * A map of possible out-of band callbacks related to the parent operation. The key is a unique identifier for the Callback Object. Each value in the map is a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#callback-object Callback Object} that describes a request that may be initiated by the API provider and the expected responses.
   */
  callbacks?: Record<string, CallbackObject$1 | ReferenceObject$1>;
  /**
   * Declares this operation to be deprecated. Consumers SHOULD refrain from usage of the declared operation. Default value is `false`.
   */
  deprecated?: boolean;
  /**
   * A verbose explanation of the operation behavior. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * Additional external documentation for this operation.
   */
  externalDocs?: ExternalDocumentationObject$1;
  /**
   * Unique string used to identify the operation. The id MUST be unique among all operations described in the API. The operationId value is **case-sensitive**. Tools and libraries MAY use the operationId to uniquely identify an operation, therefore, it is RECOMMENDED to follow common programming naming conventions.
   */
  operationId?: string;
  /**
   * A list of parameters that are applicable for this operation. If a parameter is already defined in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#path-item-parameters Path Item}, the new definition will override it but can never remove it. The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-name name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-in location}. The list can use the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#reference-object Reference Object} to link to parameters that are defined in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#components-parameters OpenAPI Object's `components.parameters`}.
   */
  parameters?: Array<ParameterObject$1 | ReferenceObject$1>;
  /**
   * The request body applicable for this operation. The `requestBody` is only supported in HTTP methods where the HTTP 1.1 specification {@link https://tools.ietf.org/html/rfc7231#section-4.3.1 RFC7231} has explicitly defined semantics for request bodies. In other cases where the HTTP spec is vague (such as {@link https://tools.ietf.org/html/rfc7231#section-4.3.1 GET}, {@link https://tools.ietf.org/html/rfc7231#section-4.3.2 HEAD} and {@link https://tools.ietf.org/html/rfc7231#section-4.3.5 DELETE}), `requestBody` SHALL be ignored by consumers.
   */
  requestBody?: RequestBodyObject$1 | ReferenceObject$1;
  /**
   * **REQUIRED**. The list of possible responses as they are returned from executing this operation.
   */
  responses: ResponsesObject$1;
  /**
   * A declaration of which security mechanisms can be used for this operation. The list of values includes alternative Security Requirement Objects that can be used. Only one of the Security Requirement Objects need to be satisfied to authorize a request. To make security optional, an empty security requirement (`{}`) can be included in the array. This definition overrides any declared top-level {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#oas-security `security`}. To remove a top-level security declaration, an empty array can be used.
   */
  security?: Array<SecurityRequirementObject$1>;
  /**
   * An alternative `servers` array to service this operation. If a `servers` array is specified at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#path-item-servers Path Item Object} or {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#oas-servers OpenAPI Object} level, it will be overridden by this value.
   */
  servers?: Array<ServerObject$1>;
  /**
   * A short summary of what the operation does.
   */
  summary?: string;
  /**
   * A list of tags for API documentation control. Tags can be used for logical grouping of operations by resources or any other qualifier.
   */
  tags?: Array<string>;
  /**
   * A list of code samples associated with an operation.
   */
  'x-codeSamples'?: Array<CodeSampleObject>;
}
/**
 * Describes a single operation parameter.
 *
 * A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-name name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-in location}.
 *
 * See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-e-percent-encoding-and-form-media-types Appendix E} for a detailed examination of percent-encoding concerns, including interactions with the `application/x-www-form-urlencoded` query string format.
 *
 * **Parameter Locations**
 *
 * There are four possible parameter locations specified by the `in` field:
 *
 * - path - Used together with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#path-templating Path Templating}, where the parameter value is actually part of the operation's URL. This does not include the host or base path of the API. For example, in `/items/{itemId}`, the path parameter is `itemId`.
 * - query - Parameters that are appended to the URL. For example, in `/items?id=###`, the query parameter is `id`.
 * - header - Custom headers that are expected as part of the request. Note that {@link https://tools.ietf.org/html/rfc7230#section-3.2 RFC7230} states header names are case insensitive.
 * - cookie - Used to pass a specific cookie value to the API.
 *
 * **Fixed Fields**
 *
 * The rules for serialization of the parameter are specified in one of two ways. Parameter Objects MUST include either a `content` field or a `schema` field, but not both. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-b-data-type-conversion Appendix B} for a discussion of converting values of various types to string representations.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * Note that while `"Cookie"` as a `name` is not forbidden if `in` is `"header"`, the effect of defining a cookie parameter that way is undefined; use `in: "cookie"` instead.
 *
 * **Fixed Fields for use with schema**
 *
 * For simpler scenarios, a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-schema `schema`} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-style `style`} can describe the structure and syntax of the parameter. When `example` or `examples` are provided in conjunction with the `schema` field, the example SHOULD match the specified schema and follow the prescribed serialization strategy for the parameter. The `example` and `examples` fields are mutually exclusive, and if either is present it SHALL _override_ any `example` in the schema.
 *
 * Serializing with `schema` is NOT RECOMMENDED for `in: "cookie"` parameters, `in: "header"` parameters that use HTTP header parameters (name=value pairs following a `;`) in their values, or `in: "header"` parameters where values might have non-URL-safe characters; see {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-d-serializing-headers-and-cookies Appendix D} for details.
 *
 * See also {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-c-using-rfc6570-based-serialization Appendix C: Using RFC6570-Based Serialization} for additional guidance.
 *
 * **Fixed Fields for use with `content`**
 *
 * For more complex scenarios, the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-content `content`} field can define the media type and schema of the parameter, as well as give examples of its use. Using `content` with a `text/plain` media type is RECOMMENDED for `in: "header"` and `in: "cookie"` parameters where the `schema` strategy is not appropriate.
 *
 * **Style Values**
 *
 * In order to support common ways of serializing simple parameters, a set of `style` values are defined.
 *
 * See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-e-percent-encoding-and-form-media-types Appendix E} for a discussion of percent-encoding, including when delimiters need to be percent-encoded and options for handling collisions with percent-encoded data.
 *
 * TODO: examples
 */
interface ParameterObject$1 extends SpecExtensions {
  /**
   * If `true`, clients MAY pass a zero-length string value in place of parameters that would otherwise be omitted entirely, which the server SHOULD interpret as the parameter being unused. Default value is `false`. If {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-style `style`} is used, and if {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#style-examples behavior is _n/a_ (cannot be serialized)}, the value of `allowEmptyValue` SHALL be ignored. Interactions between this field and the parameter's {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} are implementation-defined. This field is valid only for `query` parameters. Use of this field is NOT RECOMMENDED, and it is likely to be removed in a later revision.
   */
  allowEmptyValue?: boolean;
  /**
   * When this is true, parameter values are serialized using reserved expansion, as defined by {@link https://datatracker.ietf.org/doc/html/rfc6570#section-3.2.3 RFC6570}, which allows {@link https://datatracker.ietf.org/doc/html/rfc3986#section-2.2 RFC3986's reserved character set}, as well as percent-encoded triples, to pass through unchanged, while still percent-encoding all other disallowed characters (including `%` outside of percent-encoded triples). Applications are still responsible for percent-encoding reserved characters that are {@link https://datatracker.ietf.org/doc/html/rfc3986#section-3.4 not allowed in the query string} (`[`, `]`, `#`), or have a special meaning in `application/x-www-form-urlencoded` (`-`, `&`, `+`); see Appendices {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-c-using-rfc6570-based-serialization C} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#appendix-e-percent-encoding-and-form-media-types E} for details. This field only applies to parameters with an `in` value of `query`. The default value is `false`.
   */
  allowReserved?: boolean;
  /**
   * A map containing the representations for the parameter. The key is the media type and the value describes it. The map MUST only contain one entry.
   */
  content?: Record<string, MediaTypeObject$1>;
  /**
   * Specifies that a parameter is deprecated and SHOULD be transitioned out of usage. Default value is `false`.
   */
  deprecated?: boolean;
  /**
   * A brief description of the parameter. This could contain examples of use. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * Example of the parameter's potential value; see {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#working-with-examples Working With Examples}.
   */
  example?: unknown;
  /**
   * Examples of the parameter's potential value; see {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#working-with-examples Working With Examples}.
   */
  examples?: Record<string, ExampleObject$1 | ReferenceObject$1>;
  /**
   * When this is true, parameter values of type `array` or `object` generate separate parameters for each value of the array or key-value pair of the map. For other types of parameters this field has no effect. When {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-style `style`} is `"form"`, the default value is `true`. For all other styles, the default value is `false`. Note that despite `false` being the default for `deepObject`, the combination of `false` with `deepObject` is undefined.
   */
  explode?: boolean;
  /**
   * **REQUIRED**. The location of the parameter. Possible values are `"query"`, `"header"`, `"path"` or `"cookie"`.
   */
  in: 'cookie' | 'header' | 'path' | 'query';
  /**
   * **REQUIRED**. The name of the parameter. Parameter names are _case sensitive_.
   * - If {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-in `in`} is `"path"`, the `name` field MUST correspond to a template expression occurring within the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#paths-path path} field in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#paths-object Paths Object}. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#path-templating Path Templating} for further information.
   * - If {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-in `in`} is `"header"` and the `name` field is `"Accept"`, `"Content-Type"` or `"Authorization"`, the parameter definition SHALL be ignored.
   * - For all other cases, the `name` corresponds to the parameter name used by the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-in `in`} field.
   */
  name: string;
  /**
   * Determines whether this parameter is mandatory. If the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-in parameter location} is `"path"`, this field is **REQUIRED** and its value MUST be `true`. Otherwise, the field MAY be included and its default value is `false`.
   */
  required?: boolean;
  /**
   * The schema defining the type used for the parameter.
   */
  schema?: SchemaObject$1 | ReferenceObject$1;
  /**
   * Describes how the parameter value will be serialized depending on the type of the parameter value. Default values (based on value of `in`): for `"query"` - `"form"`; for `"path"` - `"simple"`; for `"header"` - `"simple"`; for `"cookie"` - `"form"`.
   */
  style?: 'deepObject' | 'form' | 'label' | 'matrix' | 'pipeDelimited' | 'simple' | 'spaceDelimited';
}
/**
 * Describes the operations available on a single path. A Path Item MAY be empty, due to {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#security-filtering ACL constraints}. The path itself is still exposed to the documentation viewer but they will not know which operations and parameters are available.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface PathItemObject$1 extends SpecExtensions {
  /**
   * Allows for a referenced definition of this path item. The value MUST be in the form of a URL, and the referenced structure MUST be in the form of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#path-item-object Path Item Object}. In case a Path Item Object field appears both in the defined object and the referenced object, the behavior is undefined. See the rules for resolving {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#relative-references-in-urls Relative References}.
   */
  $ref?: string;
  /**
   * A definition of a DELETE operation on this path.
   */
  delete?: OperationObject$1;
  /**
   * An optional string description, intended to apply to all operations in this path. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * A definition of a GET operation on this path.
   */
  get?: OperationObject$1;
  /**
   * A definition of a HEAD operation on this path.
   */
  head?: OperationObject$1;
  /**
   * A definition of a OPTIONS operation on this path.
   */
  options?: OperationObject$1;
  /**
   * A list of parameters that are applicable for all the operations described under this path. These parameters can be overridden at the operation level, but cannot be removed there. The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-name name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#parameter-in location}. The list can use the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#reference-object Reference Object} to link to parameters that are defined in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#components-parameters OpenAPI Object's `components.parameters`}.
   */
  parameters?: Array<ParameterObject$1 | ReferenceObject$1>;
  /**
   * A definition of a PATCH operation on this path.
   */
  patch?: OperationObject$1;
  /**
   * A definition of a POST operation on this path.
   */
  post?: OperationObject$1;
  /**
   * A definition of a PUT operation on this path.
   */
  put?: OperationObject$1;
  /**
   * An alternative `servers` array to service all operations in this path. If a `servers` array is specified at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#oas-servers OpenAPI Object} level, it will be overridden by this value.
   */
  servers?: Array<ServerObject$1>;
  /**
   * An optional string summary, intended to apply to all operations in this path.
   */
  summary?: string;
  /**
   * A definition of a TRACE operation on this path.
   */
  trace?: OperationObject$1;
}
/**
 * Holds the relative paths to the individual endpoints and their operations. The path is appended to the URL from the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#server-object Server Object} in order to construct the full URL. The Paths Object MAY be empty, due to {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#security-filtering Access Control List (ACL) constraints}.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface PathsObject$1 extends SpecExtensions {
  /**
   * A relative path to an individual endpoint. The field name MUST begin with a forward slash (`/`). The path is **appended** (no relative URL resolution) to the expanded URL from the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#server-object Server Object}'s `url` field in order to construct the full URL. {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#path-templating Path templating} is allowed. When matching URLs, concrete (non-templated) paths would be matched before their templated counterparts. Templated paths with the same hierarchy but different templated names MUST NOT exist as they are identical. In case of ambiguous matching, it's up to the tooling to decide which one to use.
   */
  [path: `/${string}`]: PathItemObject$1;
}
/**
 * A simple object to allow referencing other components in the OpenAPI Description, internally and externally.
 *
 * The Reference Object is defined by {@link https://tools.ietf.org/html/draft-pbryan-zyp-json-ref-03 JSON Reference} and follows the same structure, behavior and rules.
 *
 * For this specification, reference resolution is accomplished as defined by the JSON Reference specification and not by the JSON Schema specification.
 *
 * This object cannot be extended with additional properties, and any properties added SHALL be ignored.
 *
 * **Reference Object Example**
 *
 * ```yaml
 * $ref: '#/components/schemas/Pet'
 * ```
 *
 * **Relative Schema Document Example**
 *
 * ```yaml
 * $ref: Pet.yaml
 * ```
 *
 * **Relative Documents with Embedded Schema Example**
 *
 * ```yaml
 * $ref: definitions.yaml#/Pet
 * ```
 */
interface ReferenceObject$1 {
  /**
   * **REQUIRED**. The reference string.
   */
  $ref: string;
}
/**
 * Describes a single request body.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface RequestBodyObject$1 extends SpecExtensions {
  /**
   * **REQUIRED**. The content of the request body. The key is a media type or {@link https://tools.ietf.org/html/rfc7231#appendix-D media type range} and the value describes it. For requests that match multiple keys, only the most specific key is applicable. e.g. `"text/plain"` overrides `"text/*"`
   */
  content: Record<string, MediaTypeObject$1>;
  /**
   * A brief description of the request body. This could contain examples of use. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * Determines if the request body is required in the request. Defaults to `false`.
   */
  required?: boolean;
}
/**
 * Describes a single response from an API operation, including design-time, static `links` to operations based on the response.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface ResponseObject$1 extends SpecExtensions {
  /**
   * A map containing descriptions of potential response payloads. The key is a media type or {@link https://tools.ietf.org/html/rfc7231#appendix-D media type range} and the value describes it. For responses that match multiple keys, only the most specific key is applicable. e.g. `"text/plain"` overrides `"text/*"`
   */
  content?: Record<string, MediaTypeObject$1>;
  /**
   * **REQUIRED**. A description of the response. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description: string;
  /**
   * Maps a header name to its definition. {@link https://tools.ietf.org/html/rfc7230#section-3.2 RFC7230} states header names are case insensitive. If a response header is defined with the name `"Content-Type"`, it SHALL be ignored.
   */
  headers?: Record<string, HeaderObject$1 | ReferenceObject$1>;
  /**
   * A map of operations links that can be followed from the response. The key of the map is a short name for the link, following the naming constraints of the names for {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#components-object Component Objects}.
   */
  links?: Record<string, LinkObject$1 | ReferenceObject$1>;
}
/**
 * A container for the expected responses of an operation. The container maps a HTTP response code to the expected response.
 *
 * The documentation is not necessarily expected to cover all possible HTTP response codes because they may not be known in advance. However, documentation is expected to cover a successful operation response and any known errors.
 *
 * The `default` MAY be used as a default Response Object for all HTTP codes that are not covered individually by the Responses Object.
 *
 * The Responses Object MUST contain at least one response code, and if only one response code is provided it SHOULD be the response for a successful operation call.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface ResponsesObject$1 extends SpecExtensions {
  /**
   * Any {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#http-status-codes HTTP status code} can be used as the property name, but only one property per code, to describe the expected response for that HTTP status code. A {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#reference-object Reference Object} can link to a response that is defined in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#components-responses OpenAPI Object's `components.responses`} section. This field MUST be enclosed in quotation marks (for example, "200") for compatibility between JSON and YAML. To define a range of response codes, this field MAY contain the uppercase wildcard character `X`. For example, `2XX` represents all response codes between `200` and `299`. Only the following range definitions are allowed: `1XX`, `2XX`, `3XX`, `4XX`, and `5XX`. If a response is defined using an explicit code, the explicit code definition takes precedence over the range definition for that code.
   */
  [httpStatusCode: string]: ResponseObject$1 | ReferenceObject$1 | undefined | unknown;
  /**
   * The documentation of responses other than the ones declared for specific HTTP response codes. Use this field to cover undeclared responses. A {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#reference-object Reference Object} can link to a response that the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#components-responses OpenAPI Object's `components.responses`} section defines.
   */
  default?: ResponseObject$1 | ReferenceObject$1;
}
/**
 * The Schema Object allows the definition of input and output data types. These types can be objects, but also primitives and arrays. This object is an extended subset of the [[JSON-Schema-05|JSON Schema Specification Draft Wright-00]].
 *
 * For more information about the keywords, see {@link https://tools.ietf.org/html/draft-wright-json-schema-00 JSON Schema Core} and {@link https://tools.ietf.org/html/draft-wright-json-schema-validation-00 JSON Schema Validation}. Unless stated otherwise, the keyword definitions follow those of JSON Schema and do not add any additional semantics.
 *
 * **JSON Schema Keywords**
 *
 * The following keywords are taken directly from the JSON Schema definition and follow the same specifications:
 * - title
 * - multipleOf
 * - maximum
 * - exclusiveMaximum
 * - minimum
 * - exclusiveMinimum
 * - maxLength
 * - minLength
 * - pattern (This string SHOULD be a valid regular expression, according to the {@link https://www.ecma-international.org/ecma-262/5.1/#sec-15.10.1 Ecma-262 Edition 5.1 regular expression} dialect)
 * - maxItems
 * - minItems
 * - uniqueItems
 * - maxProperties
 * - minProperties
 * - required
 * - enum
 *
 * The following keywords are taken from the JSON Schema definition but their definitions were adjusted to the OpenAPI Specification.
 *
 * - type - Value MUST be a string. Multiple types via an array are not supported.
 * - allOf - Inline or referenced schema MUST be of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} and not a standard JSON Schema.
 * - oneOf - Inline or referenced schema MUST be of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} and not a standard JSON Schema.
 * - anyOf - Inline or referenced schema MUST be of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} and not a standard JSON Schema.
 * - not - Inline or referenced schema MUST be of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} and not a standard JSON Schema.
 * - items - Value MUST be an object and not an array. Inline or referenced schema MUST be of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} and not a standard JSON Schema. `items` MUST be present if `type` is `"array"`.
 * - properties - Property definitions MUST be a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} and not a standard JSON Schema (inline or referenced).
 * - additionalProperties - Value can be boolean or object. Inline or referenced schema MUST be of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} and not a standard JSON Schema. Consistent with JSON Schema, `additionalProperties` defaults to `true`.
 * - description - {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
 * - format - See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#data-type-format Data Type Formats} for further details. While relying on JSON Schema's defined formats, the OAS offers a few additional predefined formats.
 * - default - The default value represents what would be assumed by the consumer of the input as the value of the schema if one is not provided. Unlike JSON Schema, the value MUST conform to the defined `type` for the Schema Object defined at the same level. For example, if `type` is `"string"`, then `default` can be `"foo"` but cannot be `1`.
 *
 * Alternatively, any time a Schema Object can be used, a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#reference-object Reference Object} can be used in its place. This allows referencing definitions instead of defining them inline.
 *
 * Additional keywords defined by the JSON Schema specification that are not mentioned here are strictly unsupported.
 *
 * Other than the JSON Schema subset fields, the following fields MAY be used for further schema documentation:
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * **Composition and Inheritance (Polymorphism)**
 *
 * TODO: content, examples
 */
interface SchemaObject$1 extends EnumExtensions, SpecExtensions {
  /**
   * The value of "additionalProperties" MUST be a boolean or a schema.
   *
   * If "additionalProperties" is absent, it may be considered present with an empty schema as a value.
   *
   * If "additionalProperties" is true, validation always succeeds.
   *
   * If "additionalProperties" is false, validation succeeds only if the instance is an object and all properties on the instance were covered by "properties" and/or "patternProperties".
   *
   * If "additionalProperties" is an object, validate the value as a schema to all of the properties that weren't validated by "properties" nor "patternProperties".
   *
   * Value can be boolean or object. Inline or referenced schema MUST be of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object} and not a standard JSON Schema. Consistent with JSON Schema, `additionalProperties` defaults to `true`.
   */
  additionalProperties?: boolean | SchemaObject$1 | ReferenceObject$1;
  /**
   * This keyword's value MUST be an array.  This array MUST have at least one element.
   *
   * Elements of the array MUST be objects.  Each object MUST be a valid Schema Object.
   *
   * An instance validates successfully against this keyword if it validates successfully against all schemas defined by this keyword's value.
   */
  allOf?: Array<SchemaObject$1 | ReferenceObject$1>;
  /**
   * This keyword's value MUST be an array.  This array MUST have at least one element.
   *
   * Elements of the array MUST be objects.  Each object MUST be a valid Schema Object.
   *
   * An instance validates successfully against this keyword if it validates successfully against at least one schema defined by this
   keyword's value.
   */
  anyOf?: Array<SchemaObject$1 | ReferenceObject$1>;
  /**
   * The default value represents what would be assumed by the consumer of the input as the value of the schema if one is not provided. Unlike JSON Schema, the value MUST conform to the defined `type` for the Schema Object defined at the same level. For example, if `type` is `"string"`, then `default` can be `"foo"` but cannot be `1`.
   */
  default?: unknown;
  /**
   * Specifies that a schema is deprecated and SHOULD be transitioned out of usage. Default value is `false`.
   */
  deprecated?: boolean;
  /**
   * The value of both of these keywords MUST be a string.
   *
   * Both of these keywords can be used to decorate a user interface with information about the data produced by this user interface.  A title will preferrably be short, whereas a description will provide explanation about the purpose of the instance described by this schema.
   *
   * Both of these keywords MAY be used in root schemas, and in any subschemas.
   */
  description?: string;
  /**
   * Adds support for polymorphism. The discriminator is used to determine which of a set of schemas a payload is expected to satisfy. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#composition-and-inheritance-polymorphism Composition and Inheritance} for more details.
   */
  discriminator?: DiscriminatorObject$1;
  /**
   * The value of this keyword MUST be an array.  This array SHOULD have at least one element.  Elements in the array SHOULD be unique.
   *
   * Elements in the array MAY be of any type, including null.
   *
   * An instance validates successfully against this keyword if its value is equal to one of the elements in this keyword's array value.
   */
  enum?: Array<unknown>;
  /**
   * A free-form field to include an example of an instance for this schema. To represent examples that cannot be naturally represented in JSON or YAML, a string value can be used to contain the example with escaping where necessary.
   */
  example?: unknown;
  /**
   * The value of "exclusiveMaximum" MUST be a boolean, representing whether the limit in "maximum" is exclusive or not.  An undefined value is the same as false.
   *
   * If "exclusiveMaximum" is true, then a numeric instance SHOULD NOT be equal to the value specified in "maximum".  If "exclusiveMaximum" is false (or not specified), then a numeric instance MAY be equal to the value of "maximum".
   */
  exclusiveMaximum?: boolean;
  /**
   * The value of "exclusiveMinimum" MUST be a boolean, representing whether the limit in "minimum" is exclusive or not.  An undefined value is the same as false.
   *
   * If "exclusiveMinimum" is true, then a numeric instance SHOULD NOT be equal to the value specified in "minimum".  If "exclusiveMinimum" is false (or not specified), then a numeric instance MAY be equal to the value of "minimum".
   */
  exclusiveMinimum?: boolean;
  /**
   * Additional external documentation for this schema.
   */
  externalDocs?: ExternalDocumentationObject$1;
  /**
   * While relying on JSON Schema's defined formats, the OAS offers a few additional predefined formats.
   */
  format?: Format;
  /**
   * `items` MUST be present if `type` is `"array"`.
   */
  items?: SchemaObject$1 | ReferenceObject$1;
  /**
   * The value of this keyword MUST be an integer.  This integer MUST be greater than, or equal to, 0.
   *
   * An array instance is valid against "maxItems" if its size is less than, or equal to, the value of this keyword.
   */
  maxItems?: number;
  /**
   * The value of this keyword MUST be a non-negative integer.
   *
   * The value of this keyword MUST be an integer.  This integer MUST be greater than, or equal to, 0.
   *
   * A string instance is valid against this keyword if its length is less than, or equal to, the value of this keyword.
   *
   * The length of a string instance is defined as the number of its characters as defined by {@link https://datatracker.ietf.org/doc/html/rfc7159 RFC 7159} [RFC7159].
   */
  maxLength?: number;
  /**
   * The value of this keyword MUST be an integer.  This integer MUST be greater than, or equal to, 0.
   *
   * An object instance is valid against "maxProperties" if its number of properties is less than, or equal to, the value of this keyword.
   */
  maxProperties?: number;
  /**
   * The value of "maximum" MUST be a number, representing an upper limit for a numeric instance.
   *
   * If the instance is a number, then this keyword validates if "exclusiveMaximum" is true and instance is less than the provided value, or else if the instance is less than or exactly equal to the provided value.
   */
  maximum?: number;
  /**
   * The value of this keyword MUST be an integer.  This integer MUST be greater than, or equal to, 0.
   *
   * An array instance is valid against "minItems" if its size is greater than, or equal to, the value of this keyword.
   *
   * If this keyword is not present, it may be considered present with a value of 0.
   */
  minItems?: number;
  /**
   * A string instance is valid against this keyword if its length is greater than, or equal to, the value of this keyword.
   *
   * The length of a string instance is defined as the number of its characters as defined by {@link https://datatracker.ietf.org/doc/html/rfc7159 RFC 7159} [RFC7159].
   *
   * The value of this keyword MUST be an integer.  This integer MUST be greater than, or equal to, 0.
   *
   * "minLength", if absent, may be considered as being present with integer value 0.
   */
  minLength?: number;
  /**
   * The value of this keyword MUST be an integer.  This integer MUST be greater than, or equal to, 0.
   *
   * An object instance is valid against "minProperties" if its number of properties is greater than, or equal to, the value of this keyword.
   *
   * If this keyword is not present, it may be considered present with a value of 0.
   */
  minProperties?: number;
  /**
   * The value of "minimum" MUST be a number, representing a lower limit for a numeric instance.
   *
   * If the instance is a number, then this keyword validates if "exclusiveMinimum" is true and instance is greater than the provided value, or else if the instance is greater than or exactly equal to the provided value.
   */
  minimum?: number;
  /**
   * The value of "multipleOf" MUST be a number, strictly greater than 0.
   *
   * A numeric instance is only valid if division by this keyword's value results in an integer.
   */
  multipleOf?: number;
  /**
   * This keyword's value MUST be an object.  This object MUST be a valid Schema Object.
   *
   * An instance is valid against this keyword if it fails to validate successfully against the schema defined by this keyword.
   */
  not?: SchemaObject$1 | ReferenceObject$1;
  /**
   * This keyword only takes effect if `type` is explicitly defined within the same Schema Object. A `true` value indicates that both `null` values and values of the type specified by `type` are allowed. Other Schema Object constraints retain their defined behavior, and therefore may disallow the use of `null` as a value. A `false` value leaves the specified or default `type` unmodified. The default value is `false`.
   */
  nullable?: boolean;
  /**
   * This keyword's value MUST be an array.  This array MUST have at least one element.
   *
   * Elements of the array MUST be objects.  Each object MUST be a valid Schema Object.
   *
   * An instance validates successfully against this keyword if it validates successfully against exactly one schema defined by this keyword's value.
   */
  oneOf?: Array<SchemaObject$1 | ReferenceObject$1>;
  /**
   * The value of this keyword MUST be a string.  This string SHOULD be a valid regular expression, according to the ECMA 262 regular expression dialect.
   *
   * A string instance is considered valid if the regular expression matches the instance successfully.  Recall: regular expressions are not implicitly anchored.
   */
  pattern?: string;
  /**
   * The value of "properties" MUST be an object.  Each value of this object MUST be an object, and each object MUST be a valid Schema Object.
   *
   * If absent, it can be considered the same as an empty object.
   */
  properties?: Record<string, SchemaObject$1 | ReferenceObject$1>;
  /**
   * Relevant only for Schema Object `properties` definitions. Declares the property as "read only". This means that it MAY be sent as part of a response but SHOULD NOT be sent as part of the request. If the property is marked as `readOnly` being `true` and is in the `required` list, the `required` will take effect on the response only. A property MUST NOT be marked as both `readOnly` and `writeOnly` being `true`. Default value is `false`.
   */
  readOnly?: boolean;
  /**
   * The value of this keyword MUST be an array.  This array MUST have at least one element.  Elements of this array MUST be strings, and MUST be unique.
   *
   * An object instance is valid against this keyword if its property set contains all elements in this keyword's array value.
   */
  required?: Array<string>;
  /**
   * The value of both of these keywords MUST be a string.
   *
   * Both of these keywords can be used to decorate a user interface with information about the data produced by this user interface.  A title will preferrably be short, whereas a description will provide explanation about the purpose of the instance described by this schema.
   *
   * Both of these keywords MAY be used in root schemas, and in any subschemas.
   */
  title?: string;
  /**
   * The value of this keyword MUST be a string.
   *
   * An instance matches successfully if its primitive type is one of the types defined by keyword.  Recall: "number" includes "integer".
   */
  type?: 'array' | 'boolean' | 'integer' | 'number' | 'object' | 'string';
  /**
   * The value of this keyword MUST be a boolean.
   *
   * If this keyword has boolean value false, the instance validates successfully.  If it has boolean value true, the instance validates successfully if all of its elements are unique.
   *
   * If not present, this keyword may be considered present with boolean value false.
   */
  uniqueItems?: boolean;
  /**
   * Relevant only for Schema Object `properties` definitions. Declares the property as "write only". Therefore, it MAY be sent as part of a request but SHOULD NOT be sent as part of the response. If the property is marked as `writeOnly` being `true` and is in the `required` list, the `required` will take effect on the request only. A property MUST NOT be marked as both `readOnly` and `writeOnly` being `true`. Default value is `false`.
   */
  writeOnly?: boolean;
  /**
   * This MAY be used only on property schemas. It has no effect on root schemas. Adds additional metadata to describe the XML representation of this property.
   */
  xml?: XMLObject$1;
}
/**
 * Lists the required security schemes to execute this operation. The name used for each property MUST correspond to a security scheme declared in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#security-scheme-object Security Schemes} under the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#components-object Components Object}.
 *
 * A Security Requirement Object MAY refer to multiple security schemes in which case all schemes MUST be satisfied for a request to be authorized. This enables support for scenarios where multiple query parameters or HTTP headers are required to convey security information.
 *
 * When the `security` field is defined on the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#openapi-object OpenAPI Object} or {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#operation-object Operation Object} and contains multiple Security Requirement Objects, only one of the entries in the list needs to be satisfied to authorize the request. This enables support for scenarios where the API allows multiple, independent security schemes.
 *
 * An empty Security Requirement Object (`{}`) indicates anonymous access is supported.
 *
 * TODO: examples
 */
interface SecurityRequirementObject$1 {
  /**
   * Each name MUST correspond to a security scheme which is declared in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#security-scheme-object Security Schemes} under the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#components-object Components Object}. If the security scheme is of type `"oauth2"` or `"openIdConnect"`, then the value is a list of scope names required for the execution, and the list MAY be empty if authorization does not require a specified scope. For other security scheme types, the array MUST be empty.
   */
  [name: string]: Array<string>;
}
/**
 * Defines a security scheme that can be used by the operations.
 *
 * Supported schemes are HTTP authentication, an API key (either as a header, a cookie parameter, or as a query parameter), OAuth2's common flows (implicit, password, client credentials, and authorization code) as defined in {@link https://tools.ietf.org/html/rfc6749 RFC6749}, and [[OpenID-Connect-Core]]. Please note that as of 2020, the implicit flow is about to be deprecated by {@link https://tools.ietf.org/html/draft-ietf-oauth-security-topics OAuth 2.0 Security Best Current Practice}. Recommended for most use cases is Authorization Code Grant flow with PKCE.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
type SecuritySchemeObject$1 = SpecExtensions & {
  /**
   * A description for security scheme. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
} & ({
  /**
   * **REQUIRED**. The location of the API key. Valid values are `"query"`, `"header"`, or `"cookie"`.
   */
  in: 'cookie' | 'header' | 'query';
  /**
   * **REQUIRED**. The name of the header, query or cookie parameter to be used.
   */
  name: string;
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'apiKey';
} | {
  /**
   * A hint to the client to identify how the bearer token is formatted. Bearer tokens are usually generated by an authorization server, so this information is primarily for documentation purposes.
   */
  bearerFormat?: string;
  /**
   * **REQUIRED**. The name of the HTTP Authentication scheme to be used in the {@link https://tools.ietf.org/html/rfc7235#section-5.1 Authorization header as defined in RFC7235}. The values used SHOULD be registered in the {@link https://www.iana.org/assignments/http-authschemes/http-authschemes.xhtml IANA Authentication Scheme registry}. The value is case-insensitive, as defined in {@link https://datatracker.ietf.org/doc/html/rfc7235#section-2.1 RFC7235}.
   */
  scheme: string;
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'http';
} | {
  /**
   * **REQUIRED**. An object containing configuration information for the flow types supported.
   */
  flows: OAuthFlowsObject$1;
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'oauth2';
} | {
  /**
   * **REQUIRED**. {@link https://openid.net/specs/openid-connect-discovery-1_0.html#ProviderConfig Well-known URL} to discover the [[OpenID-Connect-Discovery]] {@link https://openid.net/specs/openid-connect-discovery-1_0.html#ProviderMetadata provider metadata}.
   */
  openIdConnectUrl: string;
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'openIdConnect';
});
/**
 * An object representing a Server.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * TODO: examples
 */
interface ServerObject$1 extends SpecExtensions {
  /**
   * An optional string describing the host designated by the URL. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * **REQUIRED**. A URL to the target host. This URL supports Server Variables and MAY be relative, to indicate that the host location is relative to the location where the document containing the Server Object is being served. Variable substitutions will be made when a variable is named in `{`braces`}`.
   */
  url: string;
  /**
   * A map between a variable name and its value. The value is used for substitution in the server's URL template.
   */
  variables?: Record<string, ServerVariableObject$1>;
}
/**
 * An object representing a Server Variable for server URL template substitution.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 */
interface ServerVariableObject$1 extends SpecExtensions {
  /**
   * **REQUIRED**. The default value to use for substitution, which SHALL be sent if an alternate value is _not_ supplied. If the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#server-variable-enum `enum`} is defined, the value SHOULD exist in the enum's values. Note that this behavior is different from the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#schema-object Schema Object}'s `default` keyword, which documents the receiver's behavior rather than inserting the value into the data.
   */
  default: string;
  /**
   * An optional description for the server variable. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * An enumeration of string values to be used if the substitution options are from a limited set. The array SHOULD NOT be empty.
   */
  enum?: Array<string>;
}
/**
 * Adds metadata to a single tag that is used by the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#operation-object Operation Object}. It is not mandatory to have a Tag Object per tag defined in the Operation Object instances.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * **Tag Object Example**
 *
 * ```yaml
 * name: pet
 * description: Pets operations
 * ```
 */
interface TagObject$1 extends SpecExtensions {
  /**
   * A description for the tag. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * Additional external documentation for this tag.
   */
  externalDocs?: ExternalDocumentationObject$1;
  /**
   * **REQUIRED**. The name of the tag.
   */
  name: string;
}
/**
 * A metadata object that allows for more fine-tuned XML model definitions.
 *
 * When using arrays, XML element names are _not_ inferred (for singular/plural forms) and the `name` field SHOULD be used to add that information. See examples for expected behavior.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#specification-extensions Specification Extensions}.
 *
 * The `namespace` field is intended to match the syntax of {@link https://www.w3.org/TR/xml-names11/ XML namespaces}, although there are a few caveats:
 * - Version 3.0.3 and earlier of this specification erroneously used the term "absolute URI" instead of "non-relative URI", so authors using namespaces that include a fragment should check tooling support carefully.
 * - XML allows but discourages relative URI-references, while this specification outright forbids them.
 * - XML 1.1 allows IRIs ({@link https://datatracker.ietf.org/doc/html/rfc3987 RFC3987}) as namespaces, and specifies that namespaces are compared without any encoding or decoding, which means that IRIs encoded to meet this specification's URI syntax requirement cannot be compared to IRIs as-is.
 *
 * TODO: examples
 */
interface XMLObject$1 extends SpecExtensions {
  /**
   * Declares whether the property definition translates to an attribute instead of an element. Default value is `false`.
   */
  attribute?: boolean;
  /**
   * Replaces the name of the element/attribute used for the described schema property. When defined within `items`, it will affect the name of the individual XML elements within the list. When defined alongside `type` being `"array"` (outside the `items`), it will affect the wrapping element if and only if `wrapped` is `true`. If `wrapped` is `false`, it will be ignored.
   */
  name?: string;
  /**
   * The URI of the namespace definition. Value MUST be in the form of a non-relative URI.
   */
  namespace?: string;
  /**
   * The prefix to be used for the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md#xml-name name}.
   */
  prefix?: string;
  /**
   * MAY be used only for an array definition. Signifies whether the array is wrapped (for example, `<books><book/><book/></books>`) or unwrapped (`<book/><book/>`). Default value is `false`. The definition takes effect only when defined alongside `type` being `"array"` (outside the `items`).
   */
  wrapped?: boolean;
}
type JsonSchemaFormats = 'date-time' | 'email' | 'hostname' | 'ipv4' | 'ipv6' | 'uri' | 'uriref';
type OpenApiSchemaFormats = 'int32' | 'int64' | 'float' | 'double' | 'byte' | 'binary' | 'date' | 'date-time' | 'password';
type Format = JsonSchemaFormats | OpenApiSchemaFormats | (string & {});
declare namespace index_d_exports$3 {
  export { CallbackObject$1 as CallbackObject, ComponentsObject$1 as ComponentsObject, ContactObject$1 as ContactObject, DiscriminatorObject$1 as DiscriminatorObject, Document$2 as Document, EncodingObject$1 as EncodingObject, ExampleObject$1 as ExampleObject, ExternalDocumentationObject$1 as ExternalDocumentationObject, HeaderObject$1 as HeaderObject, InfoObject$1 as InfoObject, LicenseObject$1 as LicenseObject, LinkObject$1 as LinkObject, MediaTypeObject$1 as MediaTypeObject, OAuthFlowObject$1 as OAuthFlowObject, OAuthFlowsObject$1 as OAuthFlowsObject, OperationObject$1 as OperationObject, ParameterObject$1 as ParameterObject, PathItemObject$1 as PathItemObject, PathsObject$1 as PathsObject, ReferenceObject$1 as ReferenceObject, RequestBodyObject$1 as RequestBodyObject, ResponseObject$1 as ResponseObject, ResponsesObject$1 as ResponsesObject, SchemaObject$1 as SchemaObject, SecurityRequirementObject$1 as SecurityRequirementObject, SecuritySchemeObject$1 as SecuritySchemeObject, ServerObject$1 as ServerObject, ServerVariableObject$1 as ServerVariableObject, TagObject$1 as TagObject, XMLObject$1 as XMLObject };
}
//#endregion
//#region src/openapi/v3-1/spec.d.ts
/**
 * This is the root object of the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#openapi-document OpenAPI document}.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 */
interface Document$1 extends SpecExtensions {
  /**
   * An element to hold various schemas for the document.
   */
  components?: ComponentsObject;
  /**
   * Additional external documentation.
   */
  externalDocs?: ExternalDocumentationObject;
  /**
   * **REQUIRED**. Provides metadata about the API. The metadata MAY be used by tooling as required.
   */
  info: InfoObject;
  /**
   * The default value for the `$schema` keyword within {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#schema-object Schema Objects} contained within this OAS document. This MUST be in the form of a URI.
   */
  jsonSchemaDialect?: string;
  /**
   * **REQUIRED**. This string MUST be the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#versions version number} of the OpenAPI Specification that the OpenAPI document uses. The `openapi` field SHOULD be used by tooling to interpret the OpenAPI document. This is _not_ related to the API {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#infoVersion `info.version`} string.
   */
  openapi: '3.1.0' | '3.1.1' | '3.1.2';
  /**
   * The available paths and operations for the API.
   */
  paths?: PathsObject;
  /**
   * A declaration of which security mechanisms can be used across the API. The list of values includes alternative security requirement objects that can be used. Only one of the security requirement objects need to be satisfied to authorize a request. Individual operations can override this definition. To make security optional, an empty security requirement (`{}`) can be included in the array.
   */
  security?: Array<SecurityRequirementObject>;
  /**
   * An array of Server Objects, which provide connectivity information to a target server. If the `servers` property is not provided, or is an empty array, the default value would be a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#server-object Server Object} with a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#serverUrl url} value of `/`.
   */
  servers?: Array<ServerObject>;
  /**
   * A list of tags used by the document with additional metadata. The order of the tags can be used to reflect on their order by the parsing tools. Not all tags that are used by the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#operation-object Operation Object} must be declared. The tags that are not declared MAY be organized randomly or based on the tools' logic. Each tag name in the list MUST be unique.
   */
  tags?: Array<TagObject>;
  /**
   * The incoming webhooks that MAY be received as part of this API and that the API consumer MAY choose to implement. Closely related to the `callbacks` feature, this section describes requests initiated other than by an API call, for example by an out of band registration. The key name is a unique string to refer to each webhook, while the (optionally referenced) Path Item Object describes a request that may be initiated by the API provider and the expected responses. An {@link https://github.com/OAI/OpenAPI-Specification/blob/main/examples/v3.1/webhook-example.yaml example} is available.
   */
  webhooks?: Record<string, PathItemObject | ReferenceObject>;
}
/**
 * A map of possible out-of band callbacks related to the parent operation. Each value in the map is a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#path-item-object Path Item Object} that describes a set of requests that may be initiated by the API provider and the expected responses. The key value used to identify the path item object is an expression, evaluated at runtime, that identifies a URL to use for the callback operation.
 *
 * To describe incoming requests from the API provider independent from another API call, use the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#oasWebhooks `webhooks`} field.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * **Key Expression**
 *
 * The key that identifies the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#path-item-object Path Item Object} is a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#runtime-expressions runtime expression} that can be evaluated in the context of a runtime HTTP request/response to identify the URL to be used for the callback request. A simple example might be $request.body#/url. However, using a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#runtime-expressions runtime expression} the complete HTTP message can be accessed. This includes accessing any part of a body that a JSON Pointer {@link https://tools.ietf.org/html/rfc6901 RFC6901} can reference.
 *
 * For example, given the following HTTP request:
 *
 * ```http
 * POST /subscribe/myevent?queryUrl=https://clientdomain.com/stillrunning HTTP/1.1
 * Host: example.org
 * Content-Type: application/json
 * Content-Length: 187
 *
 * {
 *   "failedUrl" : "https://clientdomain.com/failed",
 *   "successUrls" : [
 *     "https://clientdomain.com/fast",
 *     "https://clientdomain.com/medium",
 *     "https://clientdomain.com/slow"
 *   ]
 * }
 *
 * 201 Created
 * Location: https://example.org/subscription/1
 * ```
 *
 * The following examples show how the various expressions evaluate, assuming the callback operation has a path parameter named `eventType` and a query parameter named `queryUrl`.
 *
 * | Expression | Value |
 * | -------- | ------- |
 * | $url | https://example.org/subscribe/myevent?queryUrl=https://clientdomain.com/stillrunning |
 * | $method | POST |
 * | $request.path.eventType | myevent |
 * | $request.query.queryUrl | https://clientdomain.com/stillrunning |
 * | $request.header.content-Type | application/json |
 * | $request.body#/failedUrl | https://clientdomain.com/failed |
 * | $request.body#/successUrls/2 | https://clientdomain.com/medium |
 * | $response.header.Location | https://example.org/subscription/1 |
 *
 * **Callback Object Examples**
 *
 * The following example uses the user provided `queryUrl` query string parameter to define the callback URL. This is an example of how to use a callback object to describe a WebHook callback that goes with the subscription operation to enable registering for the WebHook.
 *
 * ```yaml
 * myCallback:
 *   '{$request.query.queryUrl}':
 *     post:
 *       requestBody:
 *         description: Callback payload
 *         content:
 *           'application/json':
 *             schema:
 *               $ref: '#/components/schemas/SomePayload'
 *       responses:
 *         '200':
 *           description: callback successfully processed
 * ```
 *
 * The following example shows a callback where the server is hard-coded, but the query string parameters are populated from the `id` and `email` property in the request body.
 *
 * ```yaml
 * transactionCallback:
 *   'http://notificationServer.com?transactionId={$request.body#/id}&email={$request.body#/email}':
 *     post:
 *       requestBody:
 *         description: Callback payload
 *         content:
 *           'application/json':
 *             schema:
 *               $ref: '#/components/schemas/SomePayload'
 *       responses:
 *         '200':
 *           description: callback successfully processed
 * ```
 */
interface CallbackObject extends SpecExtensions {
  /**
   * A Path Item Object, or a reference to one, used to define a callback request and expected responses. A {@link https://github.com/OAI/OpenAPI-Specification/blob/main/examples/v3.0/callback-example.yaml complete example} is available.
   */
  [expression: string]: PathItemObject | ReferenceObject | unknown;
}
/**
 * Holds a set of reusable objects for different aspects of the OAS. All objects defined within the components object will have no effect on the API unless they are explicitly referenced from properties outside the components object.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * All the fixed fields declared above are objects that MUST use keys that match the regular expression: `^[a-zA-Z0-9\.\-_]+$`.
 *
 * Field Name Examples:
 *
 * ```
 * User
 * User_1
 * User_Name
 * user-name
 * my.org.User
 * ```
 *
 * **Components Object Example**
 *
 * ```yaml
 * components:
 *   schemas:
 *     GeneralError:
 *       type: object
 *       properties:
 *         code:
 *           type: integer
 *           format: int32
 *         message:
 *           type: string
 *     Category:
 *       type: object
 *       properties:
 *         id:
 *           type: integer
 *           format: int64
 *         name:
 *           type: string
 *     Tag:
 *       type: object
 *       properties:
 *         id:
 *           type: integer
 *           format: int64
 *         name:
 *           type: string
 *   parameters:
 *     skipParam:
 *       name: skip
 *       in: query
 *       description: number of items to skip
 *       required: true
 *       schema:
 *         type: integer
 *         format: int32
 *     limitParam:
 *       name: limit
 *       in: query
 *       description: max records to return
 *       required: true
 *       schema:
 *         type: integer
 *         format: int32
 *   responses:
 *     NotFound:
 *       description: Entity not found.
 *     IllegalInput:
 *       description: Illegal input for operation.
 *     GeneralError:
 *       description: General Error
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/GeneralError'
 *   securitySchemes:
 *     api_key:
 *       type: apiKey
 *       name: api_key
 *       in: header
 *     petstore_auth:
 *       type: oauth2
 *       flows:
 *         implicit:
 *           authorizationUrl: https://example.org/api/oauth/dialog
 *           scopes:
 *             write:pets: modify pets in your account
 *             read:pets: read your pets
 * ```
 */
interface ComponentsObject extends SpecExtensions {
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#callback-object Callback Objects}.
   */
  callbacks?: Record<string, CallbackObject | ReferenceObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#example-object Example Objects}.
   */
  examples?: Record<string, ExampleObject | ReferenceObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#header-object Header Objects}.
   */
  headers?: Record<string, HeaderObject | ReferenceObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#link-object Link Objects}.
   */
  links?: Record<string, LinkObject | ReferenceObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameter-object Parameter Objects}.
   */
  parameters?: Record<string, ParameterObject | ReferenceObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#path-item-object Path Item Object}.
   */
  pathItems?: Record<string, PathItemObject | ReferenceObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#request-body-object Request Body Objects}.
   */
  requestBodies?: Record<string, RequestBodyObject | ReferenceObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#response-object Response Objects}.
   */
  responses?: Record<string, ResponseObject | ReferenceObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#schema-object Schema Objects}.
   */
  schemas?: Record<string, SchemaObject>;
  /**
   * An object to hold reusable {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#security-scheme-object Security Scheme Objects}.
   */
  securitySchemes?: Record<string, SecuritySchemeObject | ReferenceObject>;
}
/**
 * Contact information for the exposed API.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * name: API Support
 * url: https://www.example.com/support
 * email: support@example.com
 * ```
 */
interface ContactObject extends SpecExtensions {
  /**
   * The email address of the contact person/organization. This MUST be in the form of an email address.
   */
  email?: string;
  /**
   * The identifying name of the contact person/organization.
   */
  name?: string;
  /**
   * The URL pointing to the contact information. This MUST be in the form of a URL.
   */
  url?: string;
}
/**
 * When request bodies or response payloads may be one of a number of different schemas, a `discriminator` object can be used to aid in serialization, deserialization, and validation. The discriminator is a specific object in a schema which is used to inform the consumer of the document of an alternative schema based on the value associated with it.
 *
 * When using the discriminator, _inline_ schemas will not be considered.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * The discriminator object is legal only when using one of the composite keywords `oneOf`, `anyOf`, `allOf`.
 *
 * In OAS 3.0, a response payload MAY be described to be exactly one of any number of types:
 *
 * ```yaml
 * MyResponseType:
 *   oneOf:
 *   - $ref: '#/components/schemas/Cat'
 *   - $ref: '#/components/schemas/Dog'
 *   - $ref: '#/components/schemas/Lizard'
 * ```
 *
 * which means the payload _MUST_, by validation, match exactly one of the schemas described by `Cat`, `Dog`, or `Lizard`. In this case, a discriminator MAY act as a "hint" to shortcut validation and selection of the matching schema which may be a costly operation, depending on the complexity of the schema. We can then describe exactly which field tells us which schema to use:
 *
 * ```yaml
 * MyResponseType:
 *   oneOf:
 *   - $ref: '#/components/schemas/Cat'
 *   - $ref: '#/components/schemas/Dog'
 *   - $ref: '#/components/schemas/Lizard'
 *   discriminator:
 *     propertyName: petType
 * ```
 *
 * The expectation now is that a property with name `petType` _MUST_ be present in the response payload, and the value will correspond to the name of a schema defined in the OAS document. Thus the response payload:
 *
 * ```json
 * {
 *   "id": 12345,
 *   "petType": "Cat"
 * }
 * ```
 *
 * Will indicate that the `Cat` schema be used in conjunction with this payload.
 *
 * In scenarios where the value of the discriminator field does not match the schema name or implicit mapping is not possible, an optional `mapping` definition MAY be used:
 *
 * ```yaml
 * MyResponseType:
 *   oneOf:
 *   - $ref: '#/components/schemas/Cat'
 *   - $ref: '#/components/schemas/Dog'
 *   - $ref: '#/components/schemas/Lizard'
 *   - $ref: 'https://gigantic-server.com/schemas/Monster/schema.json'
 *   discriminator:
 *     propertyName: petType
 *     mapping:
 *       dog: '#/components/schemas/Dog'
 *       monster: 'https://gigantic-server.com/schemas/Monster/schema.json'
 * ```
 *
 * Here the discriminator _value_ of `dog` will map to the schema `#/components/schemas/Dog`, rather than the default (implicit) value of `Dog`. If the discriminator _value_ does not match an implicit or explicit mapping, no schema can be determined and validation SHOULD fail. Mapping keys MUST be string values, but tooling MAY convert response values to strings for comparison.
 *
 * When used in conjunction with the `anyOf` construct, the use of the discriminator can avoid ambiguity where multiple schemas may satisfy a single payload.
 *
 * In both the `oneOf` and `anyOf` use cases, all possible schemas MUST be listed explicitly. To avoid redundancy, the discriminator MAY be added to a parent schema definition, and all schemas comprising the parent schema in an `allOf` construct may be used as an alternate schema.
 *
 * For example:
 *
 * ```yaml
 * components:
 *   schemas:
 *     Pet:
 *       type: object
 *       required:
 *       - petType
 *       properties:
 *         petType:
 *           type: string
 *       discriminator:
 *         propertyName: petType
 *         mapping:
 *           dog: Dog
 *     Cat:
 *       allOf:
 *       - $ref: '#/components/schemas/Pet'
 *       - type: object
 *         # all other properties specific to a `Cat`
 *         properties:
 *           name:
 *             type: string
 *     Dog:
 *       allOf:
 *       - $ref: '#/components/schemas/Pet'
 *       - type: object
 *         # all other properties specific to a `Dog`
 *         properties:
 *           bark:
 *             type: string
 *     Lizard:
 *       allOf:
 *       - $ref: '#/components/schemas/Pet'
 *       - type: object
 *         # all other properties specific to a `Lizard`
 *         properties:
 *           lovesRocks:
 *             type: boolean
 * ```
 *
 * a payload like this:
 *
 * ```json
 * {
 *   "petType": "Cat",
 *   "name": "misty"
 * }
 * ```
 *
 * will indicate that the `Cat` schema be used. Likewise this schema:
 *
 * ```json
 * {
 *   "petType": "dog",
 *   "bark": "soft"
 * }
 * ```
 *
 * will map to `Dog` because of the definition in the `mapping` element.
 */
interface DiscriminatorObject extends SpecExtensions {
  /**
   * An object to hold mappings between payload values and schema names or references.
   */
  mapping?: Record<string, string>;
  /**
   * **REQUIRED**. The name of the property in the payload that will hold the discriminator value.
   */
  propertyName: string;
}
/**
 * A single encoding definition applied to a single schema property.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * requestBody:
 *   content:
 *     multipart/form-data:
 *       schema:
 *         type: object
 *         properties:
 *           id:
 *             # default is text/plain
 *             type: string
 *             format: uuid
 *           address:
 *             # default is application/json
 *             type: object
 *             properties: {}
 *           historyMetadata:
 *             # need to declare XML format!
 *             description: metadata in XML format
 *             type: object
 *             properties: {}
 *           profileImage: {}
 *       encoding:
 *         historyMetadata:
 *           # require XML Content-Type in utf-8 encoding
 *           contentType: application/xml; charset=utf-8
 *         profileImage:
 *           # only accept png/jpeg
 *           contentType: image/png, image/jpeg
 *           headers:
 *             X-Rate-Limit-Limit:
 *               description: The number of allowed requests in the current period
 *               schema:
 *                 type: integer
 * ```
 */
interface EncodingObject extends SpecExtensions {
  /**
   * Determines whether the parameter value SHOULD allow reserved characters, as defined by {@link https://tools.ietf.org/html/rfc3986#section-2.2 RFC3986} `:/?#[]@!$&'()*+,;=` to be included without percent-encoding. The default value is `false`. This property SHALL be ignored if the request body media type is not `application/x-www-form-urlencoded` or `multipart/form-data`. If a value is explicitly defined, then the value of {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#encodingContentType `contentType`} (implicit or explicit) SHALL be ignored.
   */
  allowReserved?: boolean;
  /**
   * The Content-Type for encoding a specific property. Default value depends on the property type: for `object` - `application/json`; for `array` – the default is defined based on the inner type; for all other cases the default is `application/octet-stream`. The value can be a specific media type (e.g. `application/json`), a wildcard media type (e.g. `image/*`), or a comma-separated list of the two types.
   */
  contentType?: string;
  /**
   * When this is true, property values of type `array` or `object` generate separate parameters for each value of the array, or key-value-pair of the map. For other types of properties this property has no effect. When {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#encodingStyle `style`} is `form`, the default value is `true`. For all other styles, the default value is `false`. This property SHALL be ignored if the request body media type is not `application/x-www-form-urlencoded` or `multipart/form-data`. If a value is explicitly defined, then the value of {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#encodingContentType `contentType`} (implicit or explicit) SHALL be ignored.
   */
  explode?: boolean;
  /**
   * A map allowing additional information to be provided as headers, for example `Content-Disposition`. `Content-Type` is described separately and SHALL be ignored in this section. This property SHALL be ignored if the request body media type is not a `multipart`.
   */
  headers?: Record<string, HeaderObject | ReferenceObject>;
  /**
   * Describes how a specific property value will be serialized depending on its type. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameter-object Parameter Object} for details on the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterStyle `style`} property. The behavior follows the same values as `query` parameters, including default values. This property SHALL be ignored if the request body media type is not `application/x-www-form-urlencoded` or `multipart/form-data`. If a value is explicitly defined, then the value of {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#encodingContentType `contentType`} (implicit or explicit) SHALL be ignored.
   */
  style?: 'deepObject' | 'form' | 'label' | 'matrix' | 'pipeDelimited' | 'simple' | 'spaceDelimited';
}
/**
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * In all cases, the example value is expected to be compatible with the type schema of its associated value. Tooling implementations MAY choose to validate compatibility automatically, and reject the example value(s) if incompatible.
 *
 * Example Object Examples
 *
 * In a request body:
 *
 * @example
 * ```yaml
 * requestBody:
 *   content:
 *     'application/json':
 *       schema:
 *         $ref: '#/components/schemas/Address'
 *       examples:
 *         foo:
 *           summary: A foo example
 *           value: {"foo": "bar"}
 *         bar:
 *           summary: A bar example
 *           value: {"bar": "baz"}
 *     'application/xml':
 *       examples:
 *         xmlExample:
 *           summary: This is an example in XML
 *           externalValue: 'https://example.org/examples/address-example.xml'
 *     'text/plain':
 *       examples:
 *         textExample:
 *           summary: This is a text example
 *           externalValue: 'https://foo.bar/examples/address-example.txt'
 * ```
 *
 * In a parameter:
 *
 * @example
 * ```yaml
 * parameters:
 *   - name: 'zipCode'
 *     in: 'query'
 *     schema:
 *       type: 'string'
 *       format: 'zip-code'
 *     examples:
 *       zip-example:
 *         $ref: '#/components/examples/zip-example'
 * ```
 *
 * In a response:
 *
 * @example
 * ```yaml
 * responses:
 *   '200':
 *     description: your car appointment has been booked
 *     content:
 *       application/json:
 *         schema:
 *           $ref: '#/components/schemas/SuccessResponse'
 *         examples:
 *           confirmation-success:
 *             $ref: '#/components/examples/confirmation-success'
 * ```
 */
interface ExampleObject extends SpecExtensions {
  /**
   * Long description for the example. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * A URI that points to the literal example. This provides the capability to reference examples that cannot easily be included in JSON or YAML documents. The `value` field and `externalValue` field are mutually exclusive. See the rules for resolving {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#relative-references-in-uris Relative References}.
   */
  externalValue?: string;
  /**
   * Short description for the example.
   */
  summary?: string;
  /**
   * Embedded literal example. The `value` field and `externalValue` field are mutually exclusive. To represent examples of media types that cannot naturally represented in JSON or YAML, use a string value to contain the example, escaping where necessary.
   */
  value?: unknown;
}
/**
 * Allows referencing an external resource for extended documentation.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * description: Find more info here
 * url: https://example.com
 * ```
 */
interface ExternalDocumentationObject extends SpecExtensions {
  /**
   * A description of the target documentation. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * **REQUIRED**. The URL for the target documentation. This MUST be in the form of a URL.
   */
  url: string;
}
/**
 * The Header Object follows the structure of the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameter-object Parameter Object} with the following changes:
 *
 * 1. `name` MUST NOT be specified, it is given in the corresponding `headers` map.
 * 1. `in` MUST NOT be specified, it is implicitly in `header`.
 * 1. All traits that are affected by the location MUST be applicable to a location of `header` (for example, {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterStyle `style`}).
 *
 * @example
 * ```yaml
 * description: The number of allowed requests in the current period
 * schema:
 *   type: integer
 * ```
 */
type HeaderObject = Omit<ParameterObject, 'in' | 'name'>;
/**
 * The object provides metadata about the API. The metadata MAY be used by the clients if needed, and MAY be presented in editing or documentation generation tools for convenience.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * title: Sample Pet Store App
 * summary: A pet store manager.
 * description: This is a sample server for a pet store.
 * termsOfService: https://example.com/terms/
 * contact:
 *   name: API Support
 *   url: https://www.example.com/support
 *   email: support@example.com
 * license:
 *   name: Apache 2.0
 *   url: https://www.apache.org/licenses/LICENSE-2.0.html
 * version: 1.0.1
 * ```
 */
interface InfoObject extends SpecExtensions {
  /**
   * The contact information for the exposed API.
   */
  contact?: ContactObject;
  /**
   * A description of the API. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * The license information for the exposed API.
   */
  license?: LicenseObject;
  /**
   * A short summary of the API.
   */
  summary?: string;
  /**
   * A URL to the Terms of Service for the API. This MUST be in the form of a URL.
   */
  termsOfService?: string;
  /**
   * **REQUIRED**. The title of the API.
   */
  title: string;
  /**
   * **REQUIRED**. The version of the OpenAPI document (which is distinct from the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#oasVersion OpenAPI Specification version} or the API implementation version).
   */
  version: string;
}
/**
 * License information for the exposed API.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * name: Apache 2.0
 * identifier: Apache-2.0
 * ```
 */
interface LicenseObject extends SpecExtensions {
  /**
   * An {@link https://spdx.org/licenses/ SPDX} license expression for the API. The `identifier` field is mutually exclusive of the `url` field.
   */
  identifier?: string;
  /**
   * **REQUIRED**. The license name used for the API.
   */
  name: string;
  /**
   * A URL to the license used for the API. This MUST be in the form of a URL. The `url` field is mutually exclusive of the `identifier` field.
   */
  url?: string;
}
/**
 * The `Link object` represents a possible design-time link for a response. The presence of a link does not guarantee the caller's ability to successfully invoke it, rather it provides a known relationship and traversal mechanism between responses and other operations.
 *
 * Unlike _dynamic_ links (i.e. links provided in the response payload), the OAS linking mechanism does not require link information in the runtime response.
 *
 * For computing links, and providing instructions to execute them, a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#runtime-expressions runtime expression} is used for accessing values in an operation and using them as parameters while invoking the linked operation.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * A linked operation MUST be identified using either an `operationRef` or `operationId`. In the case of an `operationId`, it MUST be unique and resolved in the scope of the OAS document. Because of the potential for name clashes, the `operationRef` syntax is preferred for OpenAPI documents with external references.
 *
 * **Examples**
 *
 * Computing a link from a request operation where the `$request.path.id` is used to pass a request parameter to the linked operation.
 *
 * ```yaml
 * paths:
 *   /users/{id}:
 *     parameters:
 *     - name: id
 *       in: path
 *       required: true
 *       description: the user identifier, as userId
 *       schema:
 *         type: string
 *     get:
 *       responses:
 *         '200':
 *           description: the user being returned
 *           content:
 *             application/json:
 *               schema:
 *                 type: object
 *                 properties:
 *                   uuid: # the unique user id
 *                     type: string
 *                     format: uuid
 *           links:
 *             address:
 *               # the target link operationId
 *               operationId: getUserAddress
 *               parameters:
 *                 # get the `id` field from the request path parameter named `id`
 *                 userId: $request.path.id
 *   # the path item of the linked operation
 *   /users/{userid}/address:
 *     parameters:
 *     - name: userid
 *       in: path
 *       required: true
 *       description: the user identifier, as userId
 *       schema:
 *         type: string
 *     # linked operation
 *     get:
 *       operationId: getUserAddress
 *       responses:
 *         '200':
 *           description: the user's address
 * ```
 *
 * When a runtime expression fails to evaluate, no parameter value is passed to the target operation.
 *
 * Values from the response body can be used to drive a linked operation.
 *
 * ```yaml
 * links:
 *   address:
 *     operationId: getUserAddressByUUID
 *     parameters:
 *       # get the `uuid` field from the `uuid` field in the response body
 *       userUuid: $response.body#/uuid
 * ```
 *
 * Clients follow all links at their discretion. Neither permissions, nor the capability to make a successful call to that link, is guaranteed solely by the existence of a relationship.
 *
 * **OperationRef Examples**
 *
 * As references to `operationId` MAY NOT be possible (the `operationId` is an optional field in an {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#operation-object Operation Object}), references MAY also be made through a relative `operationRef`:
 *
 * ```yaml
 * links:
 *   UserRepositories:
 *     # returns array of '#/components/schemas/repository'
 *     operationRef: '#/paths/~12.0~1repositories~1{username}/get'
 *     parameters:
 *       username: $response.body#/username
 * ```
 *
 * or an absolute `operationRef`:
 *
 * ```yaml
 * links:
 *   UserRepositories:
 *     # returns array of '#/components/schemas/repository'
 *     operationRef: 'https://na2.gigantic-server.com/#/paths/~12.0~1repositories~1{username}/get'
 *     parameters:
 *       username: $response.body#/username
 * ```
 *
 * Note that in the use of `operationRef`, the _escaped forward-slash_ is necessary when using JSON references.
 *
 * **Runtime Expressions**
 *
 * Runtime expressions allow defining values based on information that will only be available within the HTTP message in an actual API call. This mechanism is used by {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#link-object Link Objects} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#callback-object Callback Objects}.
 *
 * The runtime expression is defined by the following {@link https://tools.ietf.org/html/rfc5234 ABNF} syntax
 *
 * ```abnf
 * expression = ( "$url" / "$method" / "$statusCode" / "$request." source / "$response." source )
 * source = ( header-reference / query-reference / path-reference / body-reference )
 * header-reference = "header." token
 * query-reference = "query." name
 * path-reference = "path." name
 * body-reference = "body" ["#" json-pointer ]
 * json-pointer    = *( "/" reference-token )
 * reference-token = *( unescaped / escaped )
 * unescaped       = %x00-2E / %x30-7D / %x7F-10FFFF
 *   ; %x2F ('/') and %x7E ('~') are excluded from 'unescaped'
 * escaped         = "~" ( "0" / "1" )
 *   ; representing '~' and '/', respectively
 * name = *( CHAR )
 * token = 1*tchar
 * tchar = "!" / "#" / "$" / "%" / "&" / "'" / "*" / "+" / "-" / "." /
 *   "^" / "_" / "`" / "|" / "~" / DIGIT / ALPHA
 * ```
 *
 * Here, `json-pointer` is taken from {@link https://tools.ietf.org/html/rfc6901 RFC6901}, `char` from {@link https://tools.ietf.org/html/rfc7159#section-7 RFC7159} and `token` from {@link https://tools.ietf.org/html/rfc7230#section-3.2.6 RFC7230}.
 *
 * The `name` identifier is case-sensitive, whereas `token` is not.
 *
 * The table below provides examples of runtime expressions and examples of their use in a value:
 *
 * **Examples**
 *
 * | Source Location | example expression | notes |
 * | -------- | ------- | ------- |
 * | HTTP Method | `$method` | The allowable values for the `$method` will be those for the HTTP operation. |
 * | Requested media type | `$request.header.accept` | |
 * | Request parameter | `$request.path.id` | Request parameters MUST be declared in the `parameters` section of the parent operation or they cannot be evaluated. This includes request headers. |
 * | Request body property | `$request.body#/user/uuid` | In operations which accept payloads, references may be made to portions of the `requestBody` or the entire body. |
 * | Request URL | `$url` | |
 * | Response value | `$response.body#/status` | In operations which return payloads, references may be made to portions of the response body or the entire body. |
 * | Response header | `$response.header.Server` | Single header values only are available |
 *
 * Runtime expressions preserve the type of the referenced value. Expressions can be embedded into string values by surrounding the expression with `{}` curly braces.
 */
interface LinkObject extends SpecExtensions {
  /**
   * A description of the link. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * The name of an _existing_, resolvable OAS operation, as defined with a unique `operationId`. This field is mutually exclusive of the `operationRef` field.
   */
  operationId?: string;
  /**
   * A relative or absolute URI reference to an OAS operation. This field is mutually exclusive of the `operationId` field, and MUST point to an {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#operation-object Operation Object}. Relative `operationRef` values MAY be used to locate an existing {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#operation-object Operation Object} in the OpenAPI definition. See the rules for resolving {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#relative-references-in-uris Relative References}.
   */
  operationRef?: string;
  /**
   * A map representing parameters to pass to an operation as specified with `operationId` or identified via `operationRef`. The key is the parameter name to be used, whereas the value can be a constant or an expression to be evaluated and passed to the linked operation. The parameter name can be qualified using the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterIn parameter location} `[{in}.]{name}` for operations that use the same parameter name in different locations (e.g. path.id).
   */
  parameters?: Record<string, unknown | string>;
  /**
   * A literal value or {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#runtime-expressions {expression}} to use as a request body when calling the target operation.
   */
  requestBody?: unknown | string;
  /**
   * A server object to be used by the target operation.
   */
  server?: ServerObject;
}
/**
 * Each Media Type Object provides schema and examples for the media type identified by its key.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * **Media Type Examples**
 *
 * @example
 * ```yaml
 * application/json:
 *   schema:
 *     $ref: "#/components/schemas/Pet"
 *   examples:
 *     cat:
 *       summary: An example of a cat
 *       value:
 *         name: Fluffy
 *         petType: Cat
 *         color: White
 *         gender: male
 *         breed: Persian
 *     dog:
 *       summary: An example of a dog with a cat's name
 *       value:
 *         name: Puma
 *         petType: Dog
 *         color: Black
 *         gender: Female
 *         breed: Mixed
 *     frog:
 *       $ref: "#/components/examples/frog-example"
 * ```
 */
interface MediaTypeObject extends SpecExtensions {
  /**
   * A map between a property name and its encoding information. The key, being the property name, MUST exist in the schema as a property. The encoding object SHALL only apply to `requestBody` objects when the media type is `multipart` or `application/x-www-form-urlencoded`.
   */
  encoding?: Record<string, EncodingObject>;
  /**
   * Example of the media type. The example object SHOULD be in the correct format as specified by the media type. The `example` field is mutually exclusive of the `examples` field. Furthermore, if referencing a `schema` which contains an example, the `example` value SHALL _override_ the example provided by the schema.
   */
  example?: unknown;
  /**
   * Examples of the media type. Each example object SHOULD match the media type and specified schema if present. The `examples` field is mutually exclusive of the `example` field. Furthermore, if referencing a `schema` which contains an example, the `examples` value SHALL _override_ the example provided by the schema.
   */
  examples?: Record<string, ExampleObject | ReferenceObject>;
  /**
   * The schema defining the content of the request, response, or parameter.
   */
  schema?: SchemaObject;
}
/**
 * Configuration details for a supported OAuth Flow
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * **OAuth Flow Object Examples**
 *
 * ```yaml
 * type: oauth2
 * flows:
 *   implicit:
 *     authorizationUrl: https://example.com/api/oauth/dialog
 *     scopes:
 *       write:pets: modify pets in your account
 *       read:pets: read your pets
 *   authorizationCode:
 *     authorizationUrl: https://example.com/api/oauth/dialog
 *     tokenUrl: https://example.com/api/oauth/token
 *     scopes:
 *       write:pets: modify pets in your account
 *       read:pets: read your pets
 * ```
 */
interface OAuthFlowObject extends SpecExtensions {
  /**
   * **REQUIRED (`"implicit"`, `"authorizationCode"`)**. The authorization URL to be used for this flow. This MUST be in the form of a URL. The OAuth2 standard requires the use of TLS.
   */
  authorizationUrl?: string;
  /**
   * The URL to be used for obtaining refresh tokens. This MUST be in the form of a URL. The OAuth2 standard requires the use of TLS.
   */
  refreshUrl?: string;
  /**
   * **REQUIRED**. The available scopes for the OAuth2 security scheme. A map between the scope name and a short description for it. The map MAY be empty.
   */
  scopes: Record<string, string>;
  /**
   * **REQUIRED (`"password"`, `"clientCredentials"`, `"authorizationCode"`)**. The token URL to be used for this flow. This MUST be in the form of a URL. The OAuth2 standard requires the use of TLS.
   */
  tokenUrl?: string;
}
/**
 * Allows configuration of the supported OAuth Flows.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 */
interface OAuthFlowsObject extends SpecExtensions {
  /**
   * Configuration for the OAuth Authorization Code flow. Previously called `accessCode` in OpenAPI 2.0.
   */
  authorizationCode?: OAuthFlowObject;
  /**
   * Configuration for the OAuth Client Credentials flow. Previously called `application` in OpenAPI 2.0.
   */
  clientCredentials?: OAuthFlowObject;
  /**
   * Configuration for the OAuth Implicit flow
   */
  implicit?: OAuthFlowObject;
  /**
   * Configuration for the OAuth Resource Owner Password flow
   */
  password?: OAuthFlowObject;
}
/**
 * Describes a single API operation on a path.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * tags:
 * - pet
 * summary: Updates a pet in the store with form data
 * operationId: updatePetWithForm
 * parameters:
 * - name: petId
 *   in: path
 *   description: ID of pet that needs to be updated
 *   required: true
 *   schema:
 *     type: string
 * requestBody:
 *   content:
 *     'application/x-www-form-urlencoded':
 *       schema:
 *         type: object
 *         properties:
 *           name:
 *             description: Updated name of the pet
 *             type: string
 *           status:
 *             description: Updated status of the pet
 *             type: string
 *         required:
 *           - status
 * responses:
 *   '200':
 *     description: Pet updated.
 *     content:
 *       'application/json': {}
 *       'application/xml': {}
 *   '405':
 *     description: Method Not Allowed
 *     content:
 *       'application/json': {}
 *       'application/xml': {}
 * security:
 * - petstore_auth:
 *   - write:pets
 *   - read:pets
 * ```
 */
interface OperationObject extends SpecExtensions {
  /**
   * A map of possible out-of band callbacks related to the parent operation. The key is a unique identifier for the Callback Object. Each value in the map is a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#callback-object Callback Object} that describes a request that may be initiated by the API provider and the expected responses.
   */
  callbacks?: Record<string, CallbackObject | ReferenceObject>;
  /**
   * Declares this operation to be deprecated. Consumers SHOULD refrain from usage of the declared operation. Default value is `false`.
   */
  deprecated?: boolean;
  /**
   * A verbose explanation of the operation behavior. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * Additional external documentation for this operation.
   */
  externalDocs?: ExternalDocumentationObject;
  /**
   * Unique string used to identify the operation. The id MUST be unique among all operations described in the API. The operationId value is **case-sensitive**. Tools and libraries MAY use the operationId to uniquely identify an operation, therefore, it is RECOMMENDED to follow common programming naming conventions.
   */
  operationId?: string;
  /**
   * A list of parameters that are applicable for this operation. If a parameter is already defined at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#pathItemParameters Path Item}, the new definition will override it but can never remove it. The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterName name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterIn location}. The list can use the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#reference-object Reference Object} to link to parameters that are defined at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#componentsParameters OpenAPI Object's components/parameters}.
   */
  parameters?: Array<ParameterObject | ReferenceObject>;
  /**
   * The request body applicable for this operation. The `requestBody` is fully supported in HTTP methods where the HTTP 1.1 specification {@link https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.1 RFC7231} has explicitly defined semantics for request bodies. In other cases where the HTTP spec is vague (such as {@link https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.1 GET}, {@link https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.2 HEAD} and {@link https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.5 DELETE}), `requestBody` is permitted but does not have well-defined semantics and SHOULD be avoided if possible.
   */
  requestBody?: RequestBodyObject | ReferenceObject;
  /**
   * The list of possible responses as they are returned from executing this operation.
   */
  responses?: ResponsesObject;
  /**
   * A declaration of which security mechanisms can be used for this operation. The list of values includes alternative security requirement objects that can be used. Only one of the security requirement objects need to be satisfied to authorize a request. To make security optional, an empty security requirement (`{}`) can be included in the array. This definition overrides any declared top-level {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#oasSecurity `security`}. To remove a top-level security declaration, an empty array can be used.
   */
  security?: Array<SecurityRequirementObject>;
  /**
   * An alternative `server` array to service this operation. If an alternative `server` object is specified at the Path Item Object or Root level, it will be overridden by this value.
   */
  servers?: Array<ServerObject>;
  /**
   * A short summary of what the operation does.
   */
  summary?: string;
  /**
   * A list of tags for API documentation control. Tags can be used for logical grouping of operations by resources or any other qualifier.
   */
  tags?: Array<string>;
  /**
   * A list of code samples associated with an operation.
   */
  'x-codeSamples'?: Array<CodeSampleObject>;
}
/**
 * Describes a single operation parameter.
 *
 * A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterName name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterIn location}.
 *
 * **Parameter Locations**
 *
 * There are four possible parameter locations specified by the `in` field:
 *
 * - path - Used together with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#path-templating Path Templating}, where the parameter value is actually part of the operation's URL. This does not include the host or base path of the API. For example, in `/items/{itemId}`, the path parameter is `itemId`.
 * - query - Parameters that are appended to the URL. For example, in `/items?id=###`, the query parameter is `id`.
 * - header - Custom headers that are expected as part of the request. Note that {@link https://datatracker.ietf.org/doc/html/rfc7230#page-22 RFC7230} states header names are case insensitive.
 * - cookie - Used to pass a specific cookie value to the API.
 *
 * The rules for serialization of the parameter are specified in one of two ways. For simpler scenarios, a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterSchema `schema`} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterStyle `style`} can describe the structure and syntax of the parameter.
 *
 * For more complex scenarios, the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterContent `content`} property can define the media type and schema of the parameter. A parameter MUST contain either a `schema` property, or a `content` property, but not both. When `example` or `examples` are provided in conjunction with the `schema` object, the example MUST follow the prescribed serialization strategy for the parameter.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * A header parameter with an array of 64 bit integer numbers:
 *
 * @example
 * ```yaml
 * name: token
 * in: header
 * description: token to be passed as a header
 * required: true
 * schema:
 *   type: array
 *   items:
 *     type: integer
 *     format: int64
 * style: simple
 * ```
 *
 * A path parameter of a string value:
 *
 * @example
 * ```yaml
 * name: username
 * in: path
 * description: username to fetch
 * required: true
 * schema:
 *   type: string
 * ```
 *
 * An optional query parameter of a string value, allowing multiple values by repeating the query parameter:
 *
 * @example
 * ```yaml
 * name: id
 * in: query
 * description: ID of the object to fetch
 * required: false
 * schema:
 *   type: array
 *   items:
 *     type: string
 * style: form
 * explode: true
 * ```
 *
 * A free-form query parameter, allowing undefined parameters of a specific type:
 *
 * @example
 * ```yaml
 * in: query
 * name: freeForm
 * schema:
 *   type: object
 *   additionalProperties:
 *     type: integer
 * style: form
 * ```
 *
 * A complex parameter using `content` to define serialization:
 *
 * @example
 * ```yaml
 * in: query
 * name: coordinates
 * content:
 *   application/json:
 *     schema:
 *       type: object
 *       required:
 *         - lat
 *         - long
 *       properties:
 *         lat:
 *           type: number
 *         long:
 *           type: number
 * ```
 */
interface ParameterObject extends SpecExtensions {
  /**
   * Sets the ability to pass empty-valued parameters. This is valid only for `query` parameters and allows sending a parameter with an empty value. Default value is `false`. If {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterStyle `style`} is used, and if behavior is `n/a` (cannot be serialized), the value of `allowEmptyValue` SHALL be ignored. Use of this property is NOT RECOMMENDED, as it is likely to be removed in a later revision.
   */
  allowEmptyValue?: boolean;
  /**
   * Determines whether the parameter value SHOULD allow reserved characters, as defined by {@link https://datatracker.ietf.org/doc/html/rfc3986#section-2.2 RFC3986} `:/?#[]@!$&'()*+,;=` to be included without percent-encoding. This property only applies to parameters with an `in` value of `query`. The default value is `false`.
   */
  allowReserved?: boolean;
  /**
   * A map containing the representations for the parameter. The key is the media type and the value describes it. The map MUST only contain one entry.
   */
  content?: Record<string, MediaTypeObject>;
  /**
   * Specifies that a parameter is deprecated and SHOULD be transitioned out of usage. Default value is `false`.
   */
  deprecated?: boolean;
  /**
   * A brief description of the parameter. This could contain examples of use. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * Example of the parameter's potential value. The example SHOULD match the specified schema and encoding properties if present. The `example` field is mutually exclusive of the `examples` field. Furthermore, if referencing a `schema` that contains an example, the `example` value SHALL _override_ the example provided by the schema. To represent examples of media types that cannot naturally be represented in JSON or YAML, a string value can contain the example with escaping where necessary.
   */
  example?: unknown;
  /**
   * Examples of the parameter's potential value. Each example SHOULD contain a value in the correct format as specified in the parameter encoding. The `examples` field is mutually exclusive of the `example` field. Furthermore, if referencing a `schema` that contains an example, the `examples` value SHALL _override_ the example provided by the schema.
   */
  examples?: Record<string, ExampleObject | ReferenceObject>;
  /**
   * When this is true, parameter values of type `array` or `object` generate separate parameters for each value of the array or key-value pair of the map. For other types of parameters this property has no effect. When {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterStyle `style`} is `form`, the default value is `true`. For all other styles, the default value is `false`.
   */
  explode?: boolean;
  /**
   * **REQUIRED**. The location of the parameter. Possible values are `"query"`, `"header"`, `"path"` or `"cookie"`.
   */
  in: 'cookie' | 'header' | 'path' | 'query';
  /**
   * **REQUIRED**. The name of the parameter. Parameter names are _case sensitive_.
   * - If {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterIn `in`} is `"path"`, the `name` field MUST correspond to a template expression occurring within the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#pathsPath path} field in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#paths-object Paths Object}. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#path-templating Path Templating} for further information.
   * - If {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterIn `in`} is `"header"` and the `name` field is `"Accept"`, `"Content-Type"` or `"Authorization"`, the parameter definition SHALL be ignored.
   * - For all other cases, the `name` corresponds to the parameter name used by the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterIn `in`} property.
   */
  name: string;
  /**
   * Determines whether this parameter is mandatory. If the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterIn parameter location} is `"path"`, this property is **REQUIRED** and its value MUST be `true`. Otherwise, the property MAY be included and its default value is `false`.
   */
  required?: boolean;
  /**
   * The schema defining the type used for the parameter.
   */
  schema?: SchemaObject;
  /**
   * Describes how the parameter value will be serialized depending on the type of the parameter value. Default values (based on value of `in`): for `query` - `form`; for `path` - `simple`; for `header` - `simple`; for `cookie` - `form`.
   */
  style?: 'deepObject' | 'form' | 'label' | 'matrix' | 'pipeDelimited' | 'simple' | 'spaceDelimited';
}
/**
 * Describes the operations available on a single path. A Path Item MAY be empty, due to {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#security-filtering ACL constraints}. The path itself is still exposed to the documentation viewer but they will not know which operations and parameters are available.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * get:
 *   description: Returns pets based on ID
 *   summary: Find pets by ID
 *   operationId: getPetsById
 *   responses:
 *     '200':
 *       description: pet response
 *       content:
 *         '*\/*':
 *           schema:
 *             type: array
 *             items:
 *               $ref: '#/components/schemas/Pet'
 *     default:
 *       description: error payload
 *       content:
 *         'text/html':
 *           schema:
 *             $ref: '#/components/schemas/ErrorModel'
 * parameters:
 * - name: id
 *   in: path
 *   description: ID of pet to use
 *   required: true
 *   schema:
 *     type: array
 *     items:
 *       type: string
 *   style: simple
 * ```
 */
interface PathItemObject extends SpecExtensions {
  /**
   * Allows for a referenced definition of this path item. The referenced structure MUST be in the form of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#path-item-object Path Item Object}. In case a Path Item Object field appears both in the defined object and the referenced object, the behavior is undefined. See the rules for resolving {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#relative-references-in-uris Relative References}.
   */
  $ref?: string;
  /**
   * A definition of a DELETE operation on this path.
   */
  delete?: OperationObject;
  /**
   * An optional, string description, intended to apply to all operations in this path. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * A definition of a GET operation on this path.
   */
  get?: OperationObject;
  /**
   * A definition of a HEAD operation on this path.
   */
  head?: OperationObject;
  /**
   * A definition of a OPTIONS operation on this path.
   */
  options?: OperationObject;
  /**
   * A list of parameters that are applicable for all the operations described under this path. These parameters can be overridden at the operation level, but cannot be removed there. The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterName name} and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#parameterIn location}. The list can use the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#reference-object Reference Object} to link to parameters that are defined at the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#componentsParameters OpenAPI Object's components/parameters}.
   */
  parameters?: Array<ParameterObject | ReferenceObject>;
  /**
   * A definition of a PATCH operation on this path.
   */
  patch?: OperationObject;
  /**
   * A definition of a POST operation on this path.
   */
  post?: OperationObject;
  /**
   * A definition of a PUT operation on this path.
   */
  put?: OperationObject;
  /**
   * An alternative `server` array to service all operations in this path.
   */
  servers?: Array<ServerObject>;
  /**
   * An optional, string summary, intended to apply to all operations in this path.
   */
  summary?: string;
  /**
   * A definition of a TRACE operation on this path.
   */
  trace?: OperationObject;
}
/**
 * Holds the relative paths to the individual endpoints and their operations. The path is appended to the URL from the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#server-object `Server Object`} in order to construct the full URL. The Paths MAY be empty, due to {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#security-filtering Access Control List (ACL) constraints}.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * **Path Templating Matching**
 *
 * Assuming the following paths, the concrete definition, `/pets/mine`, will be matched first if used:
 *
 * ```
 * /pets/{petId}
 * /pets/mine
 * ```
 *
 * The following paths are considered identical and invalid:
 *
 * ```
 * /pets/{petId}
 * /pets/{name}
 * ```
 *
 * The following may lead to ambiguous resolution:
 *
 * ```
 * /{entity}/me
 * /books/{id}
 * ```
 *
 * **Paths Object Example**
 *
 * ```yaml
 * /pets:
 *   get:
 *     description: Returns all pets from the system that the user has access to
 *     responses:
 *       '200':
 *         description: A list of pets.
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/pet'
 * ```
 */
interface PathsObject extends SpecExtensions {
  /**
   * A relative path to an individual endpoint. The field name MUST begin with a forward slash (`/`). The path is **appended** (no relative URL resolution) to the expanded URL from the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#server-object `Server Object`}'s `url` field in order to construct the full URL. {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#path-templating Path templating} is allowed. When matching URLs, concrete (non-templated) paths would be matched before their templated counterparts. Templated paths with the same hierarchy but different templated names MUST NOT exist as they are identical. In case of ambiguous matching, it's up to the tooling to decide which one to use.
   */
  [path: `/${string}`]: PathItemObject;
}
/**
 * A simple object to allow referencing other components in the OpenAPI document, internally and externally.
 *
 * The `$ref` string value contains a URI {@link https://datatracker.ietf.org/doc/html/rfc3986 RFC3986}, which identifies the location of the value being referenced.
 *
 * See the rules for resolving {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#relative-references-in-uris Relative References}.
 *
 * This object cannot be extended with additional properties and any properties added SHALL be ignored.
 *
 * Note that this restriction on additional properties is a difference between Reference Objects and {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#schema-object `Schema Objects`} that contain a `$ref` keyword.
 *
 * Reference Object Example
 *
 * @example
 * ```yaml
 * $ref: '#/components/schemas/Pet'
 * ```
 *
 * Relative Schema Document Example
 *
 * @example
 * ```yaml
 * $ref: Pet.yaml
 * ```
 *
 * Relative Documents With Embedded Schema Example
 *
 * @example
 * ```yaml
 * $ref: definitions.yaml#/Pet
 * ```
 */
interface ReferenceObject {
  /**
   * **REQUIRED**. The reference identifier. This MUST be in the form of a URI.
   */
  $ref: string;
  /**
   * A description which by default SHOULD override that of the referenced component. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation. If the referenced object-type does not allow a `description` field, then this field has no effect.
   */
  description?: string;
  /**
   * A short summary which by default SHOULD override that of the referenced component. If the referenced object-type does not allow a `summary` field, then this field has no effect.
   */
  summary?: string;
}
/**
 * Describes a single request body.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * **Request Body Examples**
 *
 * A request body with a referenced model definition.
 *
 * ```yaml
 * description: user to add to the system
 * content:
 *   'application/json':
 *     schema:
 *       $ref: '#/components/schemas/User'
 *     examples:
 *       user:
 *         summary: User Example
 *         externalValue: 'https://foo.bar/examples/user-example.json'
 *   'application/xml':
 *     schema:
 *       $ref: '#/components/schemas/User'
 *     examples:
 *       user:
 *         summary: User example in XML
 *         externalValue: 'https://foo.bar/examples/user-example.xml'
 *   'text/plain':
 *     examples:
 *       user:
 *         summary: User example in Plain text
 *         externalValue: 'https://foo.bar/examples/user-example.txt'
 *   '*\/*':
 *     examples:
 *       user:
 *         summary: User example in other format
 *         externalValue: 'https://foo.bar/examples/user-example.whatever'
 * ```
 *
 * A body parameter that is an array of string values:
 *
 * ```yaml
 * description: user to add to the system
 * required: true
 * content:
 *   text/plain:
 *     schema:
 *       type: array
 *       items:
 *         type: string
 * ```
 */
interface RequestBodyObject extends SpecExtensions {
  /**
   * **REQUIRED**. The content of the request body. The key is a media type or {@link https://tools.ietf.org/html/rfc7231#appendix-D media type range} and the value describes it. For requests that match multiple keys, only the most specific key is applicable. e.g. text/plain overrides text/*
   */
  content: Record<string, MediaTypeObject>;
  /**
   * A brief description of the request body. This could contain examples of use. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * Determines if the request body is required in the request. Defaults to `false`.
   */
  required?: boolean;
}
/**
 * Describes a single response from an API Operation, including design-time, static `links` to operations based on the response.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * Response of an array of a complex type:
 *
 * @example
 * ```yaml
 * description: A complex object array response
 * content:
 *   application/json:
 *     schema:
 *       type: array
 *       items:
 *         $ref: '#/components/schemas/VeryComplexType'
 * ```
 *
 * Response with a string type:
 *
 * @example
 * ```yaml
 * description: A simple string response
 * content:
 *   text/plain:
 *     schema:
 *       type: string
 * ```
 *
 * Plain text response with headers:
 *
 * @example
 * ```yaml
 * description: A simple string response
 * content:
 *   text/plain:
 *     schema:
 *       type: string
 *     example: 'whoa!'
 * headers:
 *   X-Rate-Limit-Limit:
 *     description: The number of allowed requests in the current period
 *     schema:
 *       type: integer
 *   X-Rate-Limit-Remaining:
 *     description: The number of remaining requests in the current period
 *     schema:
 *       type: integer
 *   X-Rate-Limit-Reset:
 *     description: The number of seconds left in the current period
 *     schema:
 *       type: integer
 * ```
 *
 * Response with no return value:
 *
 * @example
 * ```yaml
 * description: object created
 * ```
 */
interface ResponseObject extends SpecExtensions {
  /**
   * A map containing descriptions of potential response payloads. The key is a media type or {@link https://datatracker.ietf.org/doc/html/rfc7231#appendix-D media type range} and the value describes it. For responses that match multiple keys, only the most specific key is applicable. e.g. text/plain overrides text/*
   */
  content?: Record<string, MediaTypeObject>;
  /**
   * **REQUIRED**. A description of the response. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description: string;
  /**
   * Maps a header name to its definition. {@link https://datatracker.ietf.org/doc/html/rfc7230#page-22 RFC7230} states header names are case insensitive. If a response header is defined with the name `"Content-Type"`, it SHALL be ignored.
   */
  headers?: Record<string, HeaderObject | ReferenceObject>;
  /**
   * A map of operations links that can be followed from the response. The key of the map is a short name for the link, following the naming constraints of the names for {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#components-object Component Objects}.
   */
  links?: Record<string, LinkObject | ReferenceObject>;
}
/**
 * A container for the expected responses of an operation. The container maps a HTTP response code to the expected response.
 *
 * The documentation is not necessarily expected to cover all possible HTTP response codes because they may not be known in advance. However, documentation is expected to cover a successful operation response and any known errors.
 *
 * The `default` MAY be used as a default response object for all HTTP codes that are not covered individually by the `Responses Object`.
 *
 * The `Responses Object` MUST contain at least one response code, and if only one response code is provided it SHOULD be the response for a successful operation call.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * A 200 response for a successful operation and a default response for others (implying an error):
 *
 * @example
 * ```yaml
 * '200':
 *   description: a pet to be returned
 *   content:
 *     application/json:
 *       schema:
 *         $ref: '#/components/schemas/Pet'
 * default:
 *   description: Unexpected error
 *   content:
 *     application/json:
 *       schema:
 *         $ref: '#/components/schemas/ErrorModel'
 * ```
 */
interface ResponsesObject extends SpecExtensions {
  /**
   * Any {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#http-status-codes HTTP status code} can be used as the property name, but only one property per code, to describe the expected response for that HTTP status code. This field MUST be enclosed in quotation marks (for example, "200") for compatibility between JSON and YAML. To define a range of response codes, this field MAY contain the uppercase wildcard character `X`. For example, `2XX` represents all response codes between `[200-299]`. Only the following range definitions are allowed: `1XX`, `2XX`, `3XX`, `4XX`, and `5XX`. If a response is defined using an explicit code, the explicit code definition takes precedence over the range definition for that code.
   */
  [httpStatusCode: string]: ResponseObject | ReferenceObject | undefined | unknown;
  /**
   * The documentation of responses other than the ones declared for specific HTTP response codes. Use this field to cover undeclared responses.
   */
  default?: ResponseObject | ReferenceObject;
}
/**
 * The Schema Object allows the definition of input and output data types. These types can be objects, but also primitives and arrays. This object is a superset of the {@link https://tools.ietf.org/html/draft-bhutton-json-schema-00 JSON Schema Specification Draft 2020-12}.
 *
 * For more information about the properties, see {@link https://tools.ietf.org/html/draft-bhutton-json-schema-00 JSON Schema Core} and {@link https://tools.ietf.org/html/draft-bhutton-json-schema-validation-00 JSON Schema Validation}.
 *
 * Unless stated otherwise, the property definitions follow those of JSON Schema and do not add any additional semantics. Where JSON Schema indicates that behavior is defined by the application (e.g. for annotations), OAS also defers the definition of semantics to the application consuming the OpenAPI document.
 *
 * **Properties**
 *
 * The OpenAPI Schema Object {@link https://tools.ietf.org/html/draft-bhutton-json-schema-00#section-4.3.3 dialect} is defined as requiring the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#fixed-fields-20 OAS base vocabulary}, in addition to the vocabularies as specified in the JSON Schema draft 2020-12 {@link https://tools.ietf.org/html/draft-bhutton-json-schema-00#section-8 general purpose meta-schema}.
 *
 * The OpenAPI Schema Object dialect for this version of the specification is identified by the URI `https://spec.openapis.org/oas/3.1/dialect/base` (the "OAS dialect schema id").
 *
 * The following properties are taken from the JSON Schema specification but their definitions have been extended by the OAS:
 *
 * - description - {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
 * - format - See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#dataTypeFormat Data Type Formats} for further details. While relying on JSON Schema's defined formats, the OAS offers a few additional predefined formats.
 *
 * In addition to the JSON Schema properties comprising the OAS dialect, the Schema Object supports keywords from any other vocabularies, or entirely arbitrary properties.
 *
 * The OpenAPI Specification's base vocabulary is comprised of the following keywords:
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}, though as noted, additional properties MAY omit the `x-` prefix within this object.
 */
interface SchemaObject extends BaseDocument<SchemaObject>, OpenAPIV3_1SchemaExtensions, SpecExtensions, EnumExtensions {}
/**
 * Lists the required security schemes to execute this operation. The name used for each property MUST correspond to a security scheme declared in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#componentsSecuritySchemes Security Schemes} under the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#components-object Components Object}.
 *
 * Security Requirement Objects that contain multiple schemes require that all schemes MUST be satisfied for a request to be authorized. This enables support for scenarios where multiple query parameters or HTTP headers are required to convey security information.
 *
 * When a list of Security Requirement Objects is defined on the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#openapi-object OpenAPI Object} or {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#operation-object Operation Object}, only one of the Security Requirement Objects in the list needs to be satisfied to authorize the request.
 *
 * Non-OAuth2 Security Requirement
 *
 * @example
 * ```yaml
 * api_key: []
 * ```
 *
 * OAuth2 Security Requirement
 *
 * @example
 * ```yaml
 * petstore_auth:
 * - write:pets
 * - read:pets
 * ```
 *
 * Optional OAuth2 Security
 *
 * @example
 * ```yaml
 * security:
 * - {}
 * - petstore_auth:
 *   - write:pets
 *   - read:pets
 * ```
 */
interface SecurityRequirementObject {
  /**
   * Each name MUST correspond to a security scheme which is declared in the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#componentsSecuritySchemes Security Schemes} under the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#components-object Components Object}. If the security scheme is of type `"oauth2"` or `"openIdConnect"`, then the value is a list of scope names required for the execution, and the list MAY be empty if authorization does not require a specified scope. For other security scheme types, the array MAY contain a list of role names which are required for the execution, but are not otherwise defined or exchanged in-band.
   */
  [name: string]: Array<string>;
}
/**
 * Defines a security scheme that can be used by the operations.
 *
 * Supported schemes are HTTP authentication, an API key (either as a header, a cookie parameter or as a query parameter), mutual TLS (use of a client certificate), OAuth2's common flows (implicit, password, client credentials and authorization code) as defined in {@link https://tools.ietf.org/html/rfc6749 RFC6749}, and {@link https://tools.ietf.org/html/draft-ietf-oauth-discovery-06 OpenID Connect Discovery}. Please note that as of 2020, the implicit flow is about to be deprecated by {@link https://tools.ietf.org/html/draft-ietf-oauth-security-topics OAuth 2.0 Security Best Current Practice}. Recommended for most use case is Authorization Code Grant flow with PKCE.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * **Security Scheme Object Example**
 *
 * **Basic Authentication Sample**
 *
 * ```yaml
 * type: http
 * scheme: basic
 * ```
 *
 * **API Key Sample**
 *
 * ```yaml
 * type: apiKey
 * name: api_key
 * in: header
 * ```
 *
 * **JWT Bearer Sample**
 *
 * ```yaml
 * type: http
 * scheme: bearer
 * bearerFormat: JWT
 * ```
 *
 * **Implicit OAuth2 Sample**
 *
 * ```yaml
 * type: oauth2
 * flows:
 *   implicit:
 *     authorizationUrl: https://example.com/api/oauth/dialog
 *     scopes:
 *       write:pets: modify pets in your account
 *       read:pets: read your pets
 * ```
 */
type SecuritySchemeObject = SpecExtensions & {
  /**
   * A description for security scheme. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
} & ({
  /**
   * **REQUIRED**. The location of the API key. Valid values are "query", "header" or "cookie".
   */
  in: 'cookie' | 'header' | 'query';
  /**
   * **REQUIRED**. The name of the header, query or cookie parameter to be used.
   */
  name: string;
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"mutualTLS"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'apiKey';
} | {
  /**
   * A hint to the client to identify how the bearer token is formatted. Bearer tokens are usually generated by an authorization server, so this information is primarily for documentation purposes.
   */
  bearerFormat?: string;
  /**
   * **REQUIRED**. The name of the HTTP Authorization scheme to be used in the {@link https://tools.ietf.org/html/rfc7235#section-5.1 Authorization header as defined in RFC7235}. The values used SHOULD be registered in the {@link https://www.iana.org/assignments/http-authschemes/http-authschemes.xhtml IANA Authentication Scheme registry}.
   */
  scheme: string;
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"mutualTLS"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'http';
} | {
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"mutualTLS"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'mutualTLS';
} | {
  /**
   * **REQUIRED**. An object containing configuration information for the flow types supported.
   */
  flows: OAuthFlowsObject;
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"mutualTLS"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'oauth2';
} | {
  /**
   * **REQUIRED**. OpenId Connect URL to discover OAuth2 configuration values. This MUST be in the form of a URL. The OpenID Connect standard requires the use of TLS.
   */
  openIdConnectUrl: string;
  /**
   * **REQUIRED**. The type of the security scheme. Valid values are `"apiKey"`, `"http"`, `"mutualTLS"`, `"oauth2"`, `"openIdConnect"`.
   */
  type: 'openIdConnect';
});
/**
 * An object representing a Server.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * url: https://development.gigantic-server.com/v1
 * description: Development server
 * ```
 */
interface ServerObject extends SpecExtensions {
  /**
   * An optional string describing the host designated by the URL. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * **REQUIRED**. A URL to the target host. This URL supports Server Variables and MAY be relative, to indicate that the host location is relative to the location where the OpenAPI document is being served. Variable substitutions will be made when a variable is named in `{`brackets`}`.
   */
  url: string;
  /**
   * A map between a variable name and its value. The value is used for substitution in the server's URL template.
   */
  variables?: Record<string, ServerVariableObject>;
}
/**
 * An object representing a Server Variable for server URL template substitution.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 */
interface ServerVariableObject extends SpecExtensions {
  /**
   * **REQUIRED**. The default value to use for substitution, which SHALL be sent if an alternate value is _not_ supplied. Note this behavior is different than the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#schema-object Schema Object's} treatment of default values, because in those cases parameter values are optional. If the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#serverVariableEnum `enum`} is defined, the value MUST exist in the enum's values.
   */
  default: string;
  /**
   * An optional description for the server variable. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * An enumeration of string values to be used if the substitution options are from a limited set. The array MUST NOT be empty.
   */
  enum?: Array<string>;
}
/**
 * Adds metadata to a single tag that is used by the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#operation-object Operation Object}. It is not mandatory to have a Tag Object per tag defined in the Operation Object instances.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * @example
 * ```yaml
 * name: pet
 * description: Pets operations
 * ```
 */
interface TagObject extends SpecExtensions {
  /**
   * A description for the tag. {@link https://spec.commonmark.org/ CommonMark syntax} MAY be used for rich text representation.
   */
  description?: string;
  /**
   * Additional external documentation for this tag.
   */
  externalDocs?: ExternalDocumentationObject;
  /**
   * **REQUIRED**. The name of the tag.
   */
  name: string;
}
/**
 * A metadata object that allows for more fine-tuned XML model definitions.
 *
 * When using arrays, XML element names are _not_ inferred (for singular/plural forms) and the `name` property SHOULD be used to add that information. See examples for expected behavior.
 *
 * This object MAY be extended with {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#specification-extensions Specification Extensions}.
 *
 * **XML Object Examples**
 *
 * The examples of the XML object definitions are included inside a property definition of a {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#schema-object Schema Object} with a sample of the XML representation of it.
 *
 * **No XML Element**
 *
 * Basic string property:
 *
 * ```yaml
 * animals:
 *   type: string
 * ```
 *
 * ```xml
 * <animals>...</animals>
 * ```
 *
 * Basic string array property ({@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#xmlWrapped `wrapped`} is `false` by default):
 *
 * ```yaml
 * animals:
 *   type: array
 *   items:
 *     type: string
 * ```
 *
 * ```xml
 * <animals>...</animals>
 * <animals>...</animals>
 * <animals>...</animals>
 * ```
 *
 * **XML Name Replacement**
 *
 * ```yaml
 * animals:
 *   type: string
 *   xml:
 *     name: animal
 * ```
 *
 * ```xml
 * <animal>...</animal>
 * ```
 *
 * **XML Attribute, Prefix and Namespace**
 *
 * In this example, a full model definition is shown.
 *
 * ```yaml
 * Person:
 *   type: object
 *   properties:
 *     id:
 *       type: integer
 *       format: int32
 *       xml:
 *         attribute: true
 *     name:
 *       type: string
 *       xml:
 *         namespace: https://example.com/schema/sample
 *         prefix: sample
 * ```
 *
 * ```xml
 * <Person id="123">
 *   <sample:name xmlns:sample="https://example.com/schema/sample">example</sample:name>
 * </Person>
 * ```
 *
 * **XML Arrays**
 *
 * Changing the element names:
 *
 * ```yaml
 * animals:
 *   type: array
 *   items:
 *     type: string
 *     xml:
 *       name: animal
 * ```
 *
 * ```xml
 * <animal>value</animal>
 * <animal>value</animal>
 * ```
 *
 * The external `name` property has no effect on the XML:
 *
 * ```yaml
 * animals:
 *   type: array
 *   items:
 *     type: string
 *     xml:
 *       name: animal
 *   xml:
 *     name: aliens
 * ```
 *
 * ```xml
 * <animal>value</animal>
 * <animal>value</animal>
 * ```
 *
 * Even when the array is wrapped, if a name is not explicitly defined, the same name will be used both internally and externally:
 *
 * ```yaml
 * animals:
 *   type: array
 *   items:
 *     type: string
 *   xml:
 *     wrapped: true
 * ```
 *
 * ```xml
 * <animals>
 *   <animals>value</animals>
 *   <animals>value</animals>
 * </animals>
 * ```
 *
 * To overcome the naming problem in the example above, the following definition can be used:
 *
 * ```yaml
 * animals:
 *   type: array
 *   items:
 *     type: string
 *     xml:
 *       name: animal
 *   xml:
 *     wrapped: true
 * ```
 *
 * ```xml
 * <animals>
 *   <animal>value</animal>
 *   <animal>value</animal>
 * </animals>
 * ```
 *
 * Affecting both internal and external names:
 *
 * ```yaml
 * animals:
 *   type: array
 *   items:
 *     type: string
 *     xml:
 *       name: animal
 *   xml:
 *     name: aliens
 *     wrapped: true
 * ```
 *
 * ```xml
 * <aliens>
 *   <animal>value</animal>
 *   <animal>value</animal>
 * </aliens>
 * ```
 *
 * If we change the external element but not the internal ones:
 *
 * ```yaml
 * animals:
 *   type: array
 *   items:
 *     type: string
 *   xml:
 *     name: aliens
 *     wrapped: true
 * ```
 *
 * ```xml
 * <aliens>
 *   <aliens>value</aliens>
 *   <aliens>value</aliens>
 * </aliens>
 * ```
 */
interface XMLObject extends SpecExtensions {
  /**
   * Declares whether the property definition translates to an attribute instead of an element. Default value is `false`.
   */
  attribute?: boolean;
  /**
   * Replaces the name of the element/attribute used for the described schema property. When defined within `items`, it will affect the name of the individual XML elements within the list. When defined alongside `type` being `array` (outside the `items`), it will affect the wrapping element and only if `wrapped` is `true`. If `wrapped` is `false`, it will be ignored.
   */
  name?: string;
  /**
   * The URI of the namespace definition. This MUST be in the form of an absolute URI.
   */
  namespace?: string;
  /**
   * The prefix to be used for the {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#xmlName name}.
   */
  prefix?: string;
  /**
   * MAY be used only for an array definition. Signifies whether the array is wrapped (for example, `<books><book/><book/></books>`) or unwrapped (`<book/><book/>`). Default value is `false`. The definition takes effect only when defined alongside `type` being `array` (outside the `items`).
   */
  wrapped?: boolean;
}
//#endregion
//#region src/openapi/v3-1/extensions.d.ts
interface OpenAPIV3_1SchemaExtensions {
  /**
   * Adds support for polymorphism. The discriminator is an object name that is used to differentiate between other schemas which may satisfy the payload description. See {@link https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#composition-and-inheritance-polymorphism Composition and Inheritance} for more details.
   */
  discriminator?: DiscriminatorObject;
  /**
   * A free-form property to include an example of an instance for this schema. To represent examples that cannot be naturally represented in JSON or YAML, a string value can be used to contain the example with escaping where necessary.
   *
   * **Deprecated**: The `example` property has been deprecated in favor of the JSON Schema `examples` keyword. Use of `example` is discouraged, and later versions of this specification may remove it.
   */
  example?: unknown;
  /**
   * Additional external documentation for this schema.
   */
  externalDocs?: ExternalDocumentationObject;
  /**
   * This MAY be used only on properties schemas. It has no effect on root schemas. Adds additional metadata to describe the XML representation of this property.
   */
  xml?: XMLObject;
}
declare namespace index_d_exports$4 {
  export { CallbackObject, ComponentsObject, ContactObject, DiscriminatorObject, Document$1 as Document, EncodingObject, ExampleObject, OpenAPIV3_1SchemaExtensions as Extensions, ExternalDocumentationObject, HeaderObject, InfoObject, LicenseObject, LinkObject, MediaTypeObject, OAuthFlowObject, OAuthFlowsObject, OperationObject, ParameterObject, PathItemObject, PathsObject, ReferenceObject, RequestBodyObject, ResponseObject, ResponsesObject, SchemaObject, SecurityRequirementObject, SecuritySchemeObject, ServerObject, ServerVariableObject, TagObject, XMLObject };
}
declare namespace union_d_exports$1 {
  export { Document };
}
/**
 * Union of all supported OpenAPI document versions.
 */
type Document = Document$3 | Document$2 | Document$1;
//#endregion
export { type CodeSampleObject, type EnumExtensions, union_d_exports as JSONSchema, type index_d_exports as JSONSchemaDraft2020_12, type index_d_exports$1 as JSONSchemaDraft4, type LinguistLanguages, type index_d_exports$2 as OpenAPIV2, type index_d_exports$3 as OpenAPIV3, type index_d_exports$4 as OpenAPIV3_1, union_d_exports$1 as OpenApi, type SpecExtensions, type WithSpecExtensions };
//# sourceMappingURL=index.d.mts.map