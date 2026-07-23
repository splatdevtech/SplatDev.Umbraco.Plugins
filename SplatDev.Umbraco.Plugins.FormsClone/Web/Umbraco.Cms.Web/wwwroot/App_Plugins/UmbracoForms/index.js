var Ya = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
};
var d = (t, e, o) => (Ya(t, e, "read from private field"), o ? o.call(t) : e.get(t)), f = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, _ = (t, e, o, r) => (Ya(t, e, "write to private field"), r ? r.call(t, o) : e.set(t, o), o);
var F = (t, e, o) => (Ya(t, e, "access private method"), o);
import { UMB_AUTH_CONTEXT as Bf } from "@umbraco-cms/backoffice/auth";
import { UmbEntityActionBase as X } from "@umbraco-cms/backoffice/entity-action";
import { UmbModalToken as $, UMB_MODAL_MANAGER_CONTEXT as D, UmbModalBaseElement as ge, umbConfirmModal as Yl } from "@umbraco-cms/backoffice/modal";
import { UmbTreeServerDataSourceBase as ma, UmbUniqueTreeStore as pa, UmbTreeRepositoryBase as ha, UMB_TREE_PICKER_MODAL_ALIAS as gd } from "@umbraco-cms/backoffice/tree";
import { UmbContextToken as M } from "@umbraco-cms/backoffice/context-api";
import { UmbDetailRepositoryBase as Se, UmbItemServerDataSourceBase as _d, UmbItemRepositoryBase as vd } from "@umbraco-cms/backoffice/repository";
import { tryExecuteAndNotify as y, tryExecute as lo } from "@umbraco-cms/backoffice/resources";
import { UmbDetailStoreBase as we, UmbItemStoreBase as Sd } from "@umbraco-cms/backoffice/store";
import { UmbId as q } from "@umbraco-cms/backoffice/id";
import { UmbWorkspaceActionBase as jf, UmbSaveWorkspaceAction as Bo } from "@umbraco-cms/backoffice/workspace";
import { customElement as h, html as n, css as C, property as p, when as m, map as Gf, state as w, queryAll as wd, repeat as Bi, unsafeHTML as co, LitElement as It } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as be } from "@umbraco-cms/backoffice/lit-element";
import { UmbSorterController as ji } from "@umbraco-cms/backoffice/sorter";
import { UMB_NOTIFICATION_CONTEXT as Fo } from "@umbraco-cms/backoffice/notification";
import { UmbContextBase as Hf, UmbControllerBase as bd } from "@umbraco-cms/backoffice/class-api";
import { UmbObjectState as xu, partialUpdateFrozenArray as qt, filterFrozenArray as Mu, appendToFrozenArray as Au, assignToFrozenObject as Yf } from "@umbraco-cms/backoffice/observable-api";
import { UMB_CURRENT_USER_CONTEXT as Fd } from "@umbraco-cms/backoffice/current-user";
import { generateAlias as Ru, blobDownload as Kf, splitStringToArray as Ed } from "@umbraco-cms/backoffice/utils";
import { firstValueFrom as Kl, first as Xf } from "@umbraco-cms/backoffice/external/rxjs";
import { umbExtensionsRegistry as Td, UmbConditionBase as $d } from "@umbraco-cms/backoffice/extension-registry";
import { UmbLocalizationController as Qf } from "@umbraco-cms/backoffice/localization-api";
import { loadManifestApi as Jf } from "@umbraco-cms/backoffice/extension-api";
import { MediaService as Zf } from "@umbraco-cms/backoffice/external/backend-api";
import { UmbTextStyles as fa } from "@umbraco-cms/backoffice/style";
import { UMB_COLLECTION_ALIAS_CONDITION as Xl, UmbDefaultCollectionContext as ey, UMB_COLLECTION_CONTEXT as ty } from "@umbraco-cms/backoffice/collection";
import { encodeFolderName as oy } from "@umbraco-cms/backoffice/router";
import { UmbEntityBulkActionBase as iy } from "@umbraco-cms/backoffice/entity-bulk-action";
import { UmbElementMixin as st } from "@umbraco-cms/backoffice/element-api";
import { UUITextareaEvent as ry, UUIRefNodeFormElement as ay, UUIRefNodeElement as sy, UUIFormControlMixin as Cd } from "@umbraco-cms/backoffice/external/uui";
import { UmbPickerInputContext as Od } from "@umbraco-cms/backoffice/picker-input";
class Iu extends Error {
  constructor(e, o, r) {
    super(r), this.name = "ApiError", this.url = o.url, this.status = o.status, this.statusText = o.statusText, this.body = o.body, this.request = e;
  }
}
class ny extends Error {
  constructor(e) {
    super(e), this.name = "CancelError";
  }
  get isCancelled() {
    return !0;
  }
}
class ly {
  constructor(e) {
    this._isResolved = !1, this._isRejected = !1, this._isCancelled = !1, this.cancelHandlers = [], this.promise = new Promise((o, r) => {
      this._resolve = o, this._reject = r;
      const i = (u) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isResolved = !0, this._resolve && this._resolve(u));
      }, a = (u) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isRejected = !0, this._reject && this._reject(u));
      }, s = (u) => {
        this._isResolved || this._isRejected || this._isCancelled || this.cancelHandlers.push(u);
      };
      return Object.defineProperty(s, "isResolved", {
        get: () => this._isResolved
      }), Object.defineProperty(s, "isRejected", {
        get: () => this._isRejected
      }), Object.defineProperty(s, "isCancelled", {
        get: () => this._isCancelled
      }), e(i, a, s);
    });
  }
  get [Symbol.toStringTag]() {
    return "Cancellable Promise";
  }
  then(e, o) {
    return this.promise.then(e, o);
  }
  catch(e) {
    return this.promise.catch(e);
  }
  finally(e) {
    return this.promise.finally(e);
  }
  cancel() {
    if (!(this._isResolved || this._isRejected || this._isCancelled)) {
      if (this._isCancelled = !0, this.cancelHandlers.length)
        try {
          for (const e of this.cancelHandlers)
            e();
        } catch (e) {
          console.warn("Cancellation threw an error", e);
          return;
        }
      this.cancelHandlers.length = 0, this._reject && this._reject(new ny("Request aborted"));
    }
  }
  get isCancelled() {
    return this._isCancelled;
  }
}
class Uu {
  constructor() {
    this._fns = [];
  }
  eject(e) {
    const o = this._fns.indexOf(e);
    o !== -1 && (this._fns = [
      ...this._fns.slice(0, o),
      ...this._fns.slice(o + 1)
    ]);
  }
  use(e) {
    this._fns = [...this._fns, e];
  }
}
const l = {
  BASE: "",
  CREDENTIALS: "include",
  ENCODE_PATH: void 0,
  HEADERS: void 0,
  PASSWORD: void 0,
  TOKEN: void 0,
  USERNAME: void 0,
  VERSION: "Latest",
  WITH_CREDENTIALS: !1,
  interceptors: {
    request: new Uu(),
    response: new Uu()
  }
};
var Ql = /* @__PURE__ */ ((t) => (t.SHOW = "Show", t.HIDE = "Hide", t))(Ql || {}), Jl = /* @__PURE__ */ ((t) => (t.ALL = "All", t.ANY = "Any", t))(Jl || {}), Pd = /* @__PURE__ */ ((t) => (t.IS = "Is", t.IS_NOT = "IsNot", t.GREATER_THEN = "GreaterThen", t.LESS_THEN = "LessThen", t.CONTAINS = "Contains", t.CONTAINS_IGNORE_CASE = "ContainsIgnoreCase", t.STARTS_WITH = "StartsWith", t.STARTS_WITH_IGNORE_CASE = "StartsWithIgnoreCase", t.ENDS_WITH = "EndsWith", t.ENDS_WITH_IGNORE_CASE = "EndsWithIgnoreCase", t.NOT_CONTAINS = "NotContains", t.NOT_CONTAINS_IGNORE_CASE = "NotContainsIgnoreCase", t.NOT_STARTS_WITH = "NotStartsWith", t.NOT_STARTS_WITH_IGNORE_CASE = "NotStartsWithIgnoreCase", t.NOT_ENDS_WITH = "NotEndsWith", t.NOT_ENDS_WITH_IGNORE_CASE = "NotEndsWithIgnoreCase", t))(Pd || {}), kd = /* @__PURE__ */ ((t) => (t.NO_INDICATOR = "NoIndicator", t.MARK_MANDATORY_FIELDS = "MarkMandatoryFields", t.MARK_OPTIONAL_FIELDS = "MarkOptionalFields", t))(kd || {}), Je = /* @__PURE__ */ ((t) => (t.FALSE = "False", t.TRUE = "True", t.UNDEFINED = "Undefined", t))(Je || {}), se = /* @__PURE__ */ ((t) => (t.NONE = "None", t.SHOW_AT_TOP = "ShowAtTop", t.SHOW_AT_BOTTOM = "ShowAtBottom", t))(se || {}), Zl = /* @__PURE__ */ ((t) => (t.ASCENDING = "Ascending", t.DESCENDING = "Descending", t))(Zl || {});
const Gi = (t) => typeof t == "string", Ka = (t) => Gi(t) && t !== "", ec = (t) => t instanceof Blob, Dd = (t) => t instanceof FormData, cy = (t) => {
  try {
    return btoa(t);
  } catch {
    return Buffer.from(t).toString("base64");
  }
}, uy = (t) => {
  const e = [], o = (i, a) => {
    e.push(`${encodeURIComponent(i)}=${encodeURIComponent(String(a))}`);
  }, r = (i, a) => {
    a != null && (Array.isArray(a) ? a.forEach((s) => r(i, s)) : typeof a == "object" ? Object.entries(a).forEach(([s, u]) => r(`${i}[${s}]`, u)) : o(i, a));
  };
  return Object.entries(t).forEach(([i, a]) => r(i, a)), e.length ? `?${e.join("&")}` : "";
}, dy = (t, e) => {
  const o = encodeURI, r = e.url.replace("{api-version}", t.VERSION).replace(/{(.*?)}/g, (a, s) => {
    var u;
    return (u = e.path) != null && u.hasOwnProperty(s) ? o(String(e.path[s])) : a;
  }), i = t.BASE + r;
  return e.query ? i + uy(e.query) : i;
}, my = (t) => {
  if (t.formData) {
    const e = new FormData(), o = (r, i) => {
      Gi(i) || ec(i) ? e.append(r, i) : e.append(r, JSON.stringify(i));
    };
    return Object.entries(t.formData).filter(([, r]) => r != null).forEach(([r, i]) => {
      Array.isArray(i) ? i.forEach((a) => o(r, a)) : o(r, i);
    }), e;
  }
}, nr = async (t, e) => typeof e == "function" ? e(t) : e, py = async (t, e) => {
  const [o, r, i, a] = await Promise.all([
    nr(e, t.TOKEN),
    nr(e, t.USERNAME),
    nr(e, t.PASSWORD),
    nr(e, t.HEADERS)
  ]), s = Object.entries({
    Accept: "application/json",
    ...a,
    ...e.headers
  }).filter(([, u]) => u != null).reduce((u, [S, g]) => ({
    ...u,
    [S]: String(g)
  }), {});
  if (Ka(o) && (s.Authorization = `Bearer ${o}`), Ka(r) && Ka(i)) {
    const u = cy(`${r}:${i}`);
    s.Authorization = `Basic ${u}`;
  }
  return e.body !== void 0 && (e.mediaType ? s["Content-Type"] = e.mediaType : ec(e.body) ? s["Content-Type"] = e.body.type || "application/octet-stream" : Gi(e.body) ? s["Content-Type"] = "text/plain" : Dd(e.body) || (s["Content-Type"] = "application/json")), new Headers(s);
}, hy = (t) => {
  var e, o;
  if (t.body !== void 0)
    return (e = t.mediaType) != null && e.includes("application/json") || (o = t.mediaType) != null && o.includes("+json") ? JSON.stringify(t.body) : Gi(t.body) || ec(t.body) || Dd(t.body) ? t.body : JSON.stringify(t.body);
}, fy = async (t, e, o, r, i, a, s) => {
  const u = new AbortController();
  let S = {
    headers: a,
    body: r ?? i,
    method: e.method,
    signal: u.signal
  };
  t.WITH_CREDENTIALS && (S.credentials = t.CREDENTIALS);
  for (const g of t.interceptors.request._fns)
    S = await g(S);
  return s(() => u.abort()), await fetch(o, S);
}, yy = (t, e) => {
  if (e) {
    const o = t.headers.get(e);
    if (Gi(o))
      return o;
  }
}, gy = async (t) => {
  if (t.status !== 204)
    try {
      const e = t.headers.get("Content-Type");
      if (e) {
        const o = ["application/octet-stream", "application/pdf", "application/zip", "audio/", "image/", "video/"];
        if (e.includes("application/json") || e.includes("+json"))
          return await t.json();
        if (o.some((r) => e.includes(r)))
          return await t.blob();
        if (e.includes("multipart/form-data"))
          return await t.formData();
        if (e.includes("text/"))
          return await t.text();
      }
    } catch (e) {
      console.error(e);
    }
}, _y = (t, e) => {
  const r = {
    400: "Bad Request",
    401: "Unauthorized",
    402: "Payment Required",
    403: "Forbidden",
    404: "Not Found",
    405: "Method Not Allowed",
    406: "Not Acceptable",
    407: "Proxy Authentication Required",
    408: "Request Timeout",
    409: "Conflict",
    410: "Gone",
    411: "Length Required",
    412: "Precondition Failed",
    413: "Payload Too Large",
    414: "URI Too Long",
    415: "Unsupported Media Type",
    416: "Range Not Satisfiable",
    417: "Expectation Failed",
    418: "Im a teapot",
    421: "Misdirected Request",
    422: "Unprocessable Content",
    423: "Locked",
    424: "Failed Dependency",
    425: "Too Early",
    426: "Upgrade Required",
    428: "Precondition Required",
    429: "Too Many Requests",
    431: "Request Header Fields Too Large",
    451: "Unavailable For Legal Reasons",
    500: "Internal Server Error",
    501: "Not Implemented",
    502: "Bad Gateway",
    503: "Service Unavailable",
    504: "Gateway Timeout",
    505: "HTTP Version Not Supported",
    506: "Variant Also Negotiates",
    507: "Insufficient Storage",
    508: "Loop Detected",
    510: "Not Extended",
    511: "Network Authentication Required",
    ...t.errors
  }[e.status];
  if (r)
    throw new Iu(t, e, r);
  if (!e.ok) {
    const i = e.status ?? "unknown", a = e.statusText ?? "unknown", s = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new Iu(
      t,
      e,
      `Generic Error: status: ${i}; status text: ${a}; body: ${s}`
    );
  }
}, c = (t, e) => new ly(async (o, r, i) => {
  try {
    const a = dy(t, e), s = my(e), u = hy(e), S = await py(t, e);
    if (!i.isCancelled) {
      let g = await fy(t, e, a, u, s, S, i);
      for (const Y of t.interceptors.response._fns)
        g = await Y(g);
      const b = await gy(g), I = yy(g, e.responseHeader), B = {
        url: a,
        ok: g.ok,
        status: g.status,
        statusText: g.statusText,
        body: I ?? b
      };
      _y(e, B), o(B.body);
    }
  } catch (a) {
    r(a);
  }
});
class vy {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getConfig() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/config",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class Sy {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getDataSourceType() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/data-source-type",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getDataSourceTypeById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/data-source-type/{id}",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
}
class Ke {
  /**
   * @returns string Created
   * @throws ApiError
   */
  static postDataSource(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/data-source",
      body: o,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getDataSource(e = {}) {
    const {
      skip: o,
      take: r
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/data-source",
      query: {
        skip: o,
        take: r
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static deleteDataSourceById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "DELETE",
      url: "/umbraco/forms/management/api/v1/data-source/{id}",
      path: {
        id: o
      },
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getDataSourceById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/data-source/{id}",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putDataSourceById(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/data-source/{id}",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getDataSourceScaffold() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/data-source/scaffold",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getDatasourceWizardByIdScaffold(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/datasource/wizard/{id}/scaffold",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static postDatasourceWizardCreateForm(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/datasource/wizard/create-form",
      body: o,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTreeDataSourceRoot() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/tree/data-source/root",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class MP {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTreeEmailTemplateChildrenByParentPath(e) {
    const {
      parentPath: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/tree/email-template/children/{parentPath}",
      path: {
        parentPath: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "The authenticated user do not have access to this resource"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTreeEmailTemplateRoot() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/tree/email-template/root",
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "The authenticated user do not have access to this resource"
      }
    });
  }
}
class is {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getExport(e = {}) {
    const {
      formId: o,
      fileName: r
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/export",
      query: {
        formId: o,
        fileName: r
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static postExport(e = {}) {
    const {
      formId: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/export",
      query: {
        formId: o
      },
      body: r,
      mediaType: "application/json",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getExportTypes(e = {}) {
    const {
      formId: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/export/types",
      query: {
        formId: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class ya {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFieldType() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/field-type",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFieldTypeById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/field-type/{id}",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns any OK
   * @throws ApiError
   */
  static getFieldTypeRichtextDatatype() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/field-type/richtext-datatype",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFieldTypeValidationPattern() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/field-type/validation-pattern",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class ht {
  /**
   * @returns string Created
   * @throws ApiError
   */
  static postFolder(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/folder",
      body: o,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static deleteFolderById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "DELETE",
      url: "/umbraco/forms/management/api/v1/folder/{id}",
      path: {
        id: o
      },
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFolderById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/folder/{id}",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putFolderById(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/folder/{id}",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns boolean OK
   * @throws ApiError
   */
  static getFolderByIdIsEmpty(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/folder/{id}/is-empty",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putFolderByIdMove(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/folder/{id}/move",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getItemFolder(e = {}) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/item/folder",
      query: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class wy {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormTemplate() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form-template",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class K {
  /**
   * @returns string Created
   * @throws ApiError
   */
  static postForm(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/form",
      body: o,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getForm() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static postFormFieldByIdValidateSettings(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/form-field/{id}/validate-settings",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static postFormWorkflowByIdValidateSettings(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/form-workflow/{id}/validate-settings",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static deleteFormById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "DELETE",
      url: "/umbraco/forms/management/api/v1/form/{id}",
      path: {
        id: o
      },
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormById(e) {
    const {
      id: o,
      applyDictionaryTranslations: r
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/{id}",
      path: {
        id: o
      },
      query: {
        applyDictionaryTranslations: r
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putFormById(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/form/{id}",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string Created
   * @throws ApiError
   */
  static postFormByIdCopy(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/form/{id}/copy",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static postFormByIdCopyWorkflows(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/form/{id}/copy-workflows",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns boolean OK
   * @throws ApiError
   */
  static getFormByIdHasRelations(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/{id}/has-relations",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putFormByIdMove(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/form/{id}/move",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormByIdRelations(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/{id}/relations",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormExport(e = {}) {
    const {
      guid: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/export",
      query: {
        guid: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static postFormImport(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/form/import",
      body: o,
      mediaType: "application/json",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormScaffold() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/scaffold",
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormScaffoldByTemplate(e) {
    const {
      template: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/scaffold/{template}",
      path: {
        template: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getItemForm(e = {}) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/item/form",
      query: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTreeFormChildrenByParentId(e) {
    const {
      parentId: o,
      foldersOnly: r,
      ignoreStartFolders: i
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/tree/form/children/{parentId}",
      path: {
        parentId: o
      },
      query: {
        foldersOnly: r,
        ignoreStartFolders: i
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTreeFormRoot(e = {}) {
    const {
      foldersOnly: o,
      ignoreStartFolders: r
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/tree/form/root",
      query: {
        foldersOnly: o,
        ignoreStartFolders: r
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class xd {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static postLicensingAvailable(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/licensing/available",
      body: o,
      mediaType: "application/json",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static postLicensingConfigure(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/licensing/configure",
      body: o,
      mediaType: "application/json",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getLicensingStatus() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/licensing/status",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class by {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getMediaByPath(e = {}) {
    const {
      path: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/media/by-path",
      query: {
        path: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
}
class AP {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPickerDataType() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/picker/data-type",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPickerDocumentType() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/picker/document-type",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPickerDocumentTypeByAliasProperties(e) {
    const {
      alias: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/picker/document-type/{alias}/properties",
      path: {
        alias: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static postPickerDocumentTypeMappingsRefresh(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/picker/document-type/mappings/refresh",
      body: o,
      mediaType: "application/json",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
}
class Fy {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPrevalueSourceType() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/prevalue-source-type",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPrevalueSourceTypeById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/prevalue-source-type/{id}",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
}
class ft {
  /**
   * @returns string Created
   * @throws ApiError
   */
  static postPrevalueSource(e = {}) {
    const {
      requestBody: o
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/prevalue-source",
      body: o,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPrevalueSource(e = {}) {
    const {
      skip: o,
      take: r
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/prevalue-source",
      query: {
        skip: o,
        take: r
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static deletePrevalueSourceById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "DELETE",
      url: "/umbraco/forms/management/api/v1/prevalue-source/{id}",
      path: {
        id: o
      },
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPrevalueSourceById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/prevalue-source/{id}",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putPrevalueSourceById(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/prevalue-source/{id}",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        500: "Internal Server Error"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPrevalueSourceByIdValues(e) {
    const {
      id: o,
      formId: r,
      fieldId: i
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/prevalue-source/{id}/values",
      path: {
        id: o
      },
      query: {
        formId: r,
        fieldId: i
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getPrevalueSourceScaffold() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/prevalue-source/scaffold",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTreePrevalueSourceRoot() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/tree/prevalue-source/root",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class Ze {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormByFormIdRecord(e) {
    const {
      formId: o,
      skip: r,
      take: i,
      memberKey: a,
      sortBy: s,
      sortOrder: u,
      startDate: S,
      endDate: g,
      filter: b,
      states: I,
      localTimeOffset: B,
      recordId: Y
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/{formId}/record",
      path: {
        formId: o
      },
      query: {
        Skip: r,
        Take: i,
        MemberKey: a,
        SortBy: s,
        SortOrder: u,
        StartDate: S,
        EndDate: g,
        Filter: b,
        States: I,
        LocalTimeOffset: B,
        RecordId: Y
      },
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putFormByFormIdRecordByRecordId(e) {
    const {
      formId: o,
      recordId: r,
      requestBody: i
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/form/{formId}/record/{recordId}",
      path: {
        formId: o,
        recordId: r
      },
      body: i,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        403: "The authenticated user do not have access to this resource",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormByFormIdRecordByRecordIdAuditTrail(e) {
    const {
      formId: o,
      recordId: r
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/{formId}/record/{recordId}/audit-trail",
      path: {
        formId: o,
        recordId: r
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormByFormIdRecordByRecordIdWorkflowAuditTrail(e) {
    const {
      formId: o,
      recordId: r
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/{formId}/record/{recordId}/workflow-audit-trail",
      path: {
        formId: o,
        recordId: r
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static postFormByFormIdRecordByRecordIdWorkflowByWorkflowIdRetry(e) {
    const {
      formId: o,
      recordId: r,
      workflowId: i
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/form/{formId}/record/{recordId}/workflow/{workflowId}/retry",
      path: {
        formId: o,
        recordId: r,
        workflowId: i
      },
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static postFormByFormIdRecordActionsByActionIdExecute(e) {
    const {
      formId: o,
      actionId: r,
      requestBody: i
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/form/{formId}/record/actions/{actionId}/execute",
      path: {
        formId: o,
        actionId: r
      },
      body: i,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        403: "Forbidden",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getFormByFormIdRecordMetadata(e) {
    const {
      formId: o,
      skip: r,
      take: i,
      memberKey: a,
      sortBy: s,
      sortOrder: u,
      startDate: S,
      endDate: g,
      filter: b,
      states: I,
      localTimeOffset: B,
      recordId: Y
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/{formId}/record/metadata",
      path: {
        formId: o
      },
      query: {
        Skip: r,
        Take: i,
        MemberKey: a,
        SortBy: s,
        SortOrder: u,
        StartDate: S,
        EndDate: g,
        Filter: b,
        States: I,
        LocalTimeOffset: B,
        RecordId: Y
      },
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns number OK
   * @returns void No Content
   * @throws ApiError
   */
  static getFormByFormIdRecordPageNumber(e) {
    const {
      formId: o,
      skip: r,
      take: i,
      memberKey: a,
      sortBy: s,
      sortOrder: u,
      startDate: S,
      endDate: g,
      filter: b,
      states: I,
      localTimeOffset: B,
      recordId: Y
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/form/{formId}/record/page-number",
      path: {
        formId: o
      },
      query: {
        Skip: r,
        Take: i,
        MemberKey: a,
        SortBy: s,
        SortOrder: u,
        StartDate: S,
        EndDate: g,
        Filter: b,
        States: I,
        LocalTimeOffset: B,
        RecordId: Y
      },
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getRecordSetActions() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/record-set-actions",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class ie {
  /**
   * @returns string Created
   * @throws ApiError
   */
  static postSecurityUserGroupByIdFormSecurity(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/security/user-group/{id}/form-security",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static deleteSecurityUserGroupByIdFormSecurity(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "DELETE",
      url: "/umbraco/forms/management/api/v1/security/user-group/{id}/form-security",
      path: {
        id: o
      },
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getSecurityUserGroupByIdFormSecurity(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/security/user-group/{id}/form-security",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putSecurityUserGroupByIdFormSecurity(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/security/user-group/{id}/form-security",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string Created
   * @throws ApiError
   */
  static postSecurityUserByIdFormSecurity(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "POST",
      url: "/umbraco/forms/management/api/v1/security/user/{id}/form-security",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static deleteSecurityUserByIdFormSecurity(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "DELETE",
      url: "/umbraco/forms/management/api/v1/security/user/{id}/form-security",
      path: {
        id: o
      },
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getSecurityUserByIdFormSecurity(e) {
    const {
      id: o,
      explicitOnly: r
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/security/user/{id}/form-security",
      path: {
        id: o
      },
      query: {
        explicitOnly: r
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static putSecurityUserByIdFormSecurity(e) {
    const {
      id: o,
      requestBody: r
    } = e;
    return c(l, {
      method: "PUT",
      url: "/umbraco/forms/management/api/v1/security/user/{id}/form-security",
      path: {
        id: o
      },
      body: r,
      mediaType: "application/json",
      responseHeader: "Umb-Notifications",
      errors: {
        400: "Bad Request",
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getSecurityUserCurrentFormSecurity(e = {}) {
    const {
      includeFormFieldDetails: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/security/user/current/form-security",
      query: {
        includeFormFieldDetails: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getSecurityUserUsersToAssign() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/security/user/users-to-assign",
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTreeSecurityChildrenByParentId(e) {
    const {
      parentId: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/tree/security/children/{parentId}",
      path: {
        parentId: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTreeSecurityRoot() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/tree/security/root",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class Ey {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getTheme() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/theme",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class RP {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getUpdatesStatus() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/updates/status",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static getUpdatesVersion() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/updates/version",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class Md {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getWorkflowType() {
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/workflow-type",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getWorkflowTypeById(e) {
    const {
      id: o
    } = e;
    return c(l, {
      method: "GET",
      url: "/umbraco/forms/management/api/v1/workflow-type/{id}",
      path: {
        id: o
      },
      errors: {
        401: "The resource is protected and requires an authentication token",
        404: "Not Found"
      }
    });
  }
}
const Ty = [
  {
    type: "localization",
    alias: "Forms.Localization.En",
    weight: -100,
    name: "English (US)",
    meta: {
      culture: "en"
    },
    js: () => import("./en.js")
  },
  {
    type: "localization",
    alias: "Forms.Localization.Cs_CZ",
    weight: -100,
    name: "Czech",
    meta: {
      culture: "cs-cz"
    },
    js: () => import("./cs-cz.js")
  },
  {
    type: "localization",
    alias: "Forms.Localization.Da_DK",
    weight: -100,
    name: "Danish",
    meta: {
      culture: "da-dk"
    },
    js: () => import("./da-dk.js")
  },
  {
    type: "localization",
    alias: "Forms.Localization.En_GB",
    weight: -100,
    name: "English (UK)",
    meta: {
      culture: "en-gb"
    },
    js: () => import("./en-gb.js")
  },
  {
    type: "localization",
    alias: "Forms.Localization.Es_ES",
    weight: -100,
    name: "Spanish",
    meta: {
      culture: "es-es"
    },
    js: () => import("./es-es.js")
  },
  {
    type: "localization",
    alias: "Forms.Localization.Fr_FR",
    weight: -100,
    name: "French",
    meta: {
      culture: "fr-fr"
    },
    js: () => import("./fr-fr.js")
  },
  {
    type: "localization",
    alias: "Forms.Localization.It_IT",
    weight: -100,
    name: "French",
    meta: {
      culture: "it-it"
    },
    js: () => import("./it-it.js")
  },
  {
    type: "localization",
    alias: "Forms.Localization.Pl_PL",
    weight: -100,
    name: "Polish",
    meta: {
      culture: "pl-pl"
    },
    js: () => import("./pl-pl.js")
  },
  {
    type: "localization",
    alias: "Forms.Localization.Nl_NL",
    weight: -100,
    name: "Dutch",
    meta: {
      culture: "nl-nl"
    },
    js: () => import("./nl-nl.js")
  }
], $y = [...Ty], Ut = "datasource", Hi = "datasource-root", Cy = new $(
  "Forms.Modal.DataSourceCreateOptions",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class Oy extends X {
  async execute() {
    await (await this.getContext(D)).open(this, Cy, {
      data: {}
    }).onSubmit().catch(() => {
    });
  }
}
const Py = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.DataSource.Create",
    name: "Create Data Source Entity Action",
    weight: 1e3,
    api: Oy,
    forEntityTypes: [Hi],
    meta: {
      icon: "icon-add",
      label: "Create..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.DataSourceCreateOptions",
    name: "Data Source Create Options Modal",
    js: () => import("./datasource-create-options-modal.element.js")
  }
], ky = [...Py], Dy = new $(
  "Forms.Modal.DatasourceDeleteConfirm",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class xy extends X {
  async execute() {
    await (await this.getContext(D)).open(this, Dy, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const My = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Datasource.Delete",
    name: "Delete Datasource Entity Action",
    weight: 100,
    api: xy,
    forEntityTypes: [Ut],
    meta: {
      icon: "icon-delete",
      label: "Delete..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.DatasourceDeleteConfirm",
    name: "Prevalue Source Delete Confirm Modal",
    js: () => import("./datasource-delete-confirm-modal.element.js")
  }
], Ay = [...My], Ry = [
  {
    type: "entityAction",
    kind: "reloadTreeItemChildren",
    alias: "Forms.EntityAction.DataSource.ReloadChildrenOf",
    name: "Reload Children",
    forEntityTypes: [Hi]
  }
], Iy = [...Ry, ...ky, ...Ay], Ad = "Forms.Condition.DataSourceCreated", Uy = [
  {
    type: "condition",
    name: "Security Options  Condition",
    alias: Ad,
    api: () => import("./datasource-created.condition.js")
  }
], Wy = [...Uy];
class Ly extends ma {
  constructor(e) {
    super(e, {
      getRootItems: Rd,
      getChildrenOf: zy,
      getAncestorsOf: Ny,
      mapper: qy
    });
  }
}
const Rd = (t) => Ke.getTreeDataSourceRoot(), zy = (t) => {
  if (t.parent.unique === null)
    return Rd();
  throw new Error("Not supported for the data source tree");
}, Ny = (t) => {
  throw new Error("Not supported for the data source tree");
}, qy = (t) => ({
  unique: t.id,
  parent: {
    unique: null,
    entityType: Hi
  },
  name: t.name,
  entityType: Ut,
  isFolder: t.isFolder,
  hasChildren: t.hasChildren
});
class Vy extends pa {
  constructor(e) {
    super(e, Id.toString());
  }
}
const Id = new M(
  "FormsDataSourceTreeStore"
);
class By extends ha {
  constructor(e) {
    super(e, Ly, Id);
  }
  async requestTreeRoot() {
    return { data: {
      unique: null,
      entityType: Hi,
      name: "Data Sources",
      hasChildren: !0,
      isFolder: !0
    } };
  }
}
const Ud = "Umb.Section.Forms", kt = "Umb.Menu.Forms", jy = {
  type: "section",
  alias: Ud,
  name: "Forms Section",
  weight: 0,
  meta: {
    label: "Forms",
    pathname: "forms"
  }
}, Gy = [
  {
    type: "sectionSidebarApp",
    kind: "menu",
    alias: "Umb.SectionSidebarMenu.Forms",
    name: "Forms Section Sidebar Menu Forms",
    weight: 400,
    meta: {
      label: "Forms",
      menu: kt
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: Ud
      }
    ]
  }
], Hy = [
  {
    type: "menu",
    alias: kt,
    name: "Forms Menu",
    meta: {
      label: "Forms"
    }
  }
], Yy = [
  jy,
  ...Gy,
  ...Hy
], Ky = {
  type: "menuItem",
  kind: "tree",
  alias: "Forms.MenuItem.DataSource",
  name: "Forms Data Source Menu Item",
  weight: 400,
  meta: {
    label: "Data Sources",
    entityType: Ut,
    treeAlias: "Forms.Tree.DataSources",
    menus: [kt]
  }
}, Xy = [Ky], ce = "Forms.Condition.SecurityPermission", Wd = "Forms.Repository.DataSources.Tree", Qy = "Forms.Store.DataSources.Tree", Jy = "Forms.Tree.DataSources", Zy = {
  type: "repository",
  alias: Wd,
  name: "Data Source Tree Repository",
  api: By
}, eg = {
  type: "treeStore",
  alias: Qy,
  name: "Data Source Tree Store",
  api: Vy
}, tg = {
  type: "tree",
  kind: "default",
  alias: Jy,
  name: "Data Source Tree",
  meta: {
    repositoryAlias: Wd
  },
  conditions: [
    {
      alias: ce,
      match: (t) => t.userSecurity.manageDataSources
    }
  ]
}, og = {
  type: "treeItem",
  kind: "default",
  alias: "Forms.TreeItem.DataSource",
  name: "Data Source Tree Item",
  forEntityTypes: [Hi, Ut]
}, ig = [
  Zy,
  eg,
  tg,
  og,
  ...Xy
];
class rg extends we {
  constructor(e) {
    super(
      e,
      Ld.toString()
    );
  }
}
const Ld = new M("DataSourceDetailStore");
var Ve;
class ag {
  constructor(e) {
    f(this, Ve, void 0);
    _(this, Ve, e);
  }
  /**
   * Creates a new data source scaffold
   * @param {(string | null)} parentUnique
   * @return { DataSourceDetailModel }
   * @memberof FormsDataSourceDetailServerDataSource
   */
  async createScaffold(e = {}) {
    return { data: {
      entityType: Ut,
      unique: q.new(),
      id: q.new(),
      created: "",
      name: "",
      settings: {},
      formDataSourceTypeId: "",
      valid: !1,
      updated: ""
    } };
  }
  /**
   * Fetches a data source with the given id from the server
   * @param {string} unique
   * @return {FormDataSource}
   * @memberof FormsDataSourceDetailServerDataSource
   */
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, Ve),
      Ke.getDataSourceById({
        id: e
      })
    );
    return r || !o ? { error: r } : { data: o };
  }
  /**
   * Inserts a new data source on the server
   * @param {FormDetailModel} form
   * @return {*}
   * @memberof FormsDataSourceDetailServerDataSource
   */
  async create(e) {
    if (!e)
      throw new Error("Datasource is missing");
    if (!e.unique)
      throw new Error("Datasource unique is missing");
    const { error: o } = await y(
      d(this, Ve),
      Ke.postDataSource({
        requestBody: e
      })
    );
    return o ? { error: o } : this.read(e.unique);
  }
  /**
   * Updates a data source on the server
   * @param {FormDetailModel} Form
   * @return {*}
   * @memberof FormsDataSourceDetailServerDataSource
   */
  async update(e) {
    if (!e.unique)
      throw new Error("Unique is missing");
    const { error: o } = await y(
      d(this, Ve),
      Ke.putDataSourceById({
        id: e.id,
        requestBody: e
      })
    );
    return o ? { error: o } : this.read(e.unique);
  }
  /**
   * Deletes a data source on the server
   * @param {string} unique
   * @return {*}
   * @memberof FormsDataSourceDetailServerDataSource
   */
  async delete(e) {
    if (!e)
      throw new Error("Unique is missing");
    return y(
      d(this, Ve),
      Ke.deleteDataSourceById({
        id: e
      })
    );
  }
}
Ve = new WeakMap();
class sg extends Se {
  constructor(e) {
    super(
      e,
      ag,
      Ld
    );
  }
  async requestDataSourceScaffold() {
    const { data: e, error: o } = await y(this._host, Ke.getDataSourceScaffold());
    return o || !e ? { error: o } : { data: e };
  }
  async requestDataSourceWizardScaffold(e) {
    const { data: o, error: r } = await y(this._host, Ke.getDatasourceWizardByIdScaffold({ id: e }));
    return r || !o ? { error: r } : { data: o };
  }
}
const ng = "Forms.Repository.DataSource.Detail", lg = "Forms.Store.DataSource.Detail", cg = {
  type: "repository",
  alias: ng,
  name: "Data Source Detail Repository",
  api: sg
}, ug = {
  type: "store",
  alias: lg,
  name: "Data Source Detail Store",
  api: rg
}, dg = [cg, ug], mg = [...dg], Fe = "form", Eo = "form-root", he = "form-folder", rs = "form-entry";
class pg extends ma {
  constructor(e) {
    super(e, {
      getRootItems: zd,
      getChildrenOf: hg,
      getAncestorsOf: fg,
      mapper: yg
    });
  }
}
const zd = (t) => K.getTreeFormRoot({}), hg = (t) => t.parent.unique === null ? zd() : K.getTreeFormChildrenByParentId({
  parentId: t.parent.unique
}), fg = (t) => {
  throw new Error("Not supported for the forms tree");
}, yg = (t) => {
  var e;
  return {
    unique: t.id,
    parent: {
      unique: ((e = t.parent) == null ? void 0 : e.id) || null,
      entityType: t.parent ? he : Eo
    },
    name: t.name,
    path: t.path,
    entityType: t.isFolder ? he : Fe,
    isFolder: t.isFolder,
    hasChildren: t.hasChildren
  };
};
class gg extends pa {
  constructor(e) {
    super(e, Nd.toString());
  }
}
const Nd = new M(
  "FormsFormTreeStore"
);
class _g extends ha {
  constructor(e) {
    super(e, pg, Nd);
  }
  async requestTreeRoot() {
    return { data: {
      unique: null,
      entityType: Eo,
      name: "Forms",
      hasChildren: !0,
      isFolder: !0
    } };
  }
}
const vg = {
  type: "menuItem",
  kind: "tree",
  alias: "Forms.MenuItem.Form",
  name: "Forms Form Menu Item",
  weight: 600,
  meta: {
    label: "Forms",
    entityType: Fe,
    icon: "icon-folder",
    treeAlias: "Forms.Tree.Forms",
    menus: [kt]
  }
}, Sg = [vg], qd = "Forms.Repository.Forms.Tree", wg = "Forms.Store.Forms.Tree", tc = "Forms.Tree.Forms", bg = {
  type: "repository",
  alias: qd,
  name: "Form Tree Repository",
  api: _g
}, Fg = {
  type: "treeStore",
  alias: wg,
  name: "Form Tree Store",
  api: gg
}, Eg = {
  type: "tree",
  kind: "default",
  alias: tc,
  name: "Forms Tree",
  meta: {
    repositoryAlias: qd
  }
  // No condition on this tree, as it's accessible from content for picking forms.
}, Tg = {
  type: "treeItem",
  kind: "default",
  alias: "Forms.TreeItem.Form",
  name: "Form Tree Item",
  forEntityTypes: [
    Eo,
    Fe,
    he
  ]
}, $g = [
  bg,
  Fg,
  Eg,
  Tg,
  ...Sg
], Cg = new $(
  gd,
  {
    modal: {
      type: "sidebar",
      size: "small"
    },
    data: {
      treeAlias: tc
    }
  }
), Vd = "Umb.Modal.Forms.ChooseFieldType", Bd = new $(Vd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), jd = "Umb.Modal.Forms.ChooseWorkflowType", Og = new $(jd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), Gd = "Umb.Modal.Forms.ConfigureWorkflow", Pg = new $(Gd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), Hd = "Umb.Modal.Forms.CreateFormFromDataSource", kg = new $(Hd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), Yd = "Umb.Modal.Forms.EditPage", Dg = new $(Yd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), Kd = "Umb.Modal.Forms.EditFieldset", xg = new $(Kd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), Xd = "Umb.Modal.Forms.EditField", Mg = new $(Xd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), Qd = "Umb.Modal.Forms.EditSubmitMessage", Jd = new $(Qd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), Zd = "Umb.Modal.Forms.EditWorkflow", em = new $(Zd, {
  modal: {
    type: "sidebar",
    size: "medium"
  }
}), tm = "Forms.EntryDetails.Modal", Ag = new $(tm, {
  modal: {
    type: "sidebar",
    size: "large"
  }
}), Rg = new $(
  gd,
  {
    modal: {
      type: "sidebar",
      size: "small"
    },
    data: {
      treeAlias: tc
    }
  }
), om = "Umb.Modal.Forms.ExportEntries", Ig = new $(om, {
  modal: {
    type: "sidebar",
    size: "small"
  }
});
var vi;
class Ug {
  constructor(e) {
    f(this, vi, void 0);
    _(this, vi, e);
  }
  async getCollection() {
    const { data: e, error: o } = await y(d(this, vi), ya.getFieldType());
    return o ? { error: o } : e ? { data: { items: e, total: e.length } } : { data: { items: [], total: 0 } };
  }
}
vi = new WeakMap();
var Si;
class Or {
  constructor(e) {
    f(this, Si, void 0);
    _(this, Si, new Ug(e));
  }
  async requestCollection() {
    return d(this, Si).getCollection();
  }
  destroy() {
  }
}
Si = new WeakMap();
const Wg = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  FormsFieldTypeCollectionRepository: Or,
  default: Or
}, Symbol.toStringTag, { value: "Module" }));
var Lg = Object.defineProperty, zg = Object.getOwnPropertyDescriptor, Ng = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? zg(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Lg(e, o, i), i;
}, oc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, qg = (t, e, o) => (oc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), Xa = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Vg = (t, e, o, r) => (oc(t, e, "write to private field"), e.set(t, o), o), Wu = (t, e, o) => (oc(t, e, "access private method"), o), Pr, as, im, ss, rm;
const Bg = "form-choose-field-type-modal";
let kr = class extends ge {
  constructor() {
    super(), Xa(this, as), Xa(this, ss), Xa(this, Pr, []), Wu(this, as, im).call(this);
  }
  render() {
    return n`<umb-body-layout
      .headline=${this.localize.term("formEdit_chooseFieldType")}
    >
      <uui-box>
        <uui-ref-list>
          ${qg(this, Pr).map(
      (t) => n`<umb-ref-item
                selectable
                .name=${t.name}
                .detail=${t.description}
                .icon=${t.icon}
                @click=${() => Wu(this, ss, rm).call(this, t)}
              >
              </umb-ref-item>`
    )}
        </uui-ref-list>
      </uui-box>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
Pr = /* @__PURE__ */ new WeakMap();
as = /* @__PURE__ */ new WeakSet();
im = async function() {
  const t = new Or(this), { data: e } = await t.requestCollection();
  Vg(this, Pr, (e == null ? void 0 : e.items) || []), this.requestUpdate();
};
ss = /* @__PURE__ */ new WeakSet();
rm = function(t) {
  var e, o;
  (e = this.modalContext) == null || e.updateValue({ selectedValue: t }), (o = this.modalContext) == null || o.submit();
};
kr = Ng([
  h(Bg)
], kr);
const jg = kr, Gg = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsChooseFieldTypeModalElement() {
    return kr;
  },
  default: jg
}, Symbol.toStringTag, { value: "Module" }));
var wi;
class Hg {
  constructor(e) {
    f(this, wi, void 0);
    _(this, wi, e);
  }
  async getCollection() {
    const { data: e, error: o } = await y(d(this, wi), Md.getWorkflowType());
    return o ? { error: o } : e ? { data: { items: e, total: e.length } } : { data: { items: [], total: 0 } };
  }
}
wi = new WeakMap();
var bi;
class ns {
  constructor(e) {
    f(this, bi, void 0);
    _(this, bi, new Hg(e));
  }
  async requestCollection() {
    return d(this, bi).getCollection();
  }
  destroy() {
  }
}
bi = new WeakMap();
const Yg = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  FormsWorkflowTypeCollectionRepository: ns,
  default: ns
}, Symbol.toStringTag, { value: "Module" }));
var Kg = Object.defineProperty, Xg = Object.getOwnPropertyDescriptor, Qg = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? Xg(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Kg(e, o, i), i;
}, ic = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Jg = (t, e, o) => (ic(t, e, "read from private field"), o ? o.call(t) : e.get(t)), Qa = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Zg = (t, e, o, r) => (ic(t, e, "write to private field"), e.set(t, o), o), Lu = (t, e, o) => (ic(t, e, "access private method"), o), Dr, ls, am, cs, sm;
const e_ = "form-choose-workflow-type-modal";
let xr = class extends ge {
  constructor() {
    super(), Qa(this, ls), Qa(this, cs), Qa(this, Dr, []), Lu(this, ls, am).call(this);
  }
  render() {
    return n`<umb-body-layout headline=${this.localize.term("formWorkflows_chooseWorkflowType")}>
      <uui-box>
        <uui-ref-list>
          ${Jg(this, Dr).map(
      (t) => n`<umb-ref-item
                selectable
                .title=${t.name}
                .name=${t.name}
                .detail=${t.description}
                .icon=${t.icon}
                @click=${() => Lu(this, cs, sm).call(this, t)}
              >
              </umb-ref-item>`
    )}
        </uui-ref-list>
      </uui-box>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click="${this._rejectModal}"
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
Dr = /* @__PURE__ */ new WeakMap();
ls = /* @__PURE__ */ new WeakSet();
am = async function() {
  const t = new ns(this), { data: e } = await t.requestCollection();
  Zg(this, Dr, (e == null ? void 0 : e.items) || []), this.requestUpdate();
};
cs = /* @__PURE__ */ new WeakSet();
sm = function(t) {
  var e, o;
  (e = this.modalContext) == null || e.updateValue({ selectedValue: t }), (o = this.modalContext) == null || o.submit();
};
xr = Qg([
  h(e_)
], xr);
const t_ = xr, o_ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsChooseWorkflowTypeModalElement() {
    return xr;
  },
  default: t_
}, Symbol.toStringTag, { value: "Module" })), Ae = new M(
  "UmbWorkspaceContext",
  void 0,
  (t) => {
    var e;
    return ((e = t.getEntityType) == null ? void 0 : e.call(t)) === Fe;
  }
);
class ot {
  static getPageScaffold() {
    const e = q.new(), o = this.getFieldsetScaffold();
    return o.page = e, {
      caption: "",
      fieldSets: [o],
      sortOrder: 0,
      id: e,
      form: "",
      buttonCondition: null
    };
  }
  static getFieldsetScaffold() {
    return {
      caption: "",
      sortOrder: 0,
      id: q.new(),
      page: "",
      containers: [this.getContainerScaffold()],
      condition: null
    };
  }
  static getContainerScaffold() {
    return {
      id: q.new(),
      caption: "",
      width: 12,
      fields: []
    };
  }
  static getQuestionScaffold() {
    return {
      caption: "",
      alias: "",
      tooltip: "",
      id: q.new(),
      cssClass: "",
      fieldTypeId: "",
      mandatory: !1,
      prevalueSourceId: "",
      preValues: [],
      dataSourceFieldKey: "",
      condition: this.getConditionScaffold(),
      regex: "",
      requiredErrorMessage: "",
      invalidErrorMessage: "",
      containsSensitiveData: !1,
      settings: {},
      allowedUploadTypes: [],
      allowMultipleFileUploads: !1
    };
  }
  static getConditionScaffold() {
    return {
      id: q.new(),
      enabled: !1,
      actionType: Ql.SHOW,
      logicType: Jl.ALL,
      rules: []
    };
  }
  static getWorkflowScaffold() {
    return {
      id: q.new(),
      name: "",
      form: "",
      active: !0,
      includeSensitiveData: Je.FALSE,
      isDeleted: !1,
      sortOrder: 0,
      workflowTypeId: "",
      workflowTypeName: "",
      workflowTypeDescription: "",
      workflowTypeIcon: "",
      workflowTypeGroup: "",
      settings: {},
      isMandatory: !1,
      condition: null
    };
  }
}
class Yi {
  constructor(e, o, r) {
    this.config = {
      getUniqueOfElement: (i) => i.getAttribute("sort-unique"),
      getUniqueOfModel: (i) => i,
      identifier: e,
      itemSelector: o,
      containerSelector: r
    };
  }
}
var i_ = Object.defineProperty, r_ = Object.getOwnPropertyDescriptor, Re = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? r_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && i_(e, o, i), i;
}, rc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, v = (t, e, o) => (rc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), j = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Xe = (t, e, o, r) => (rc(t, e, "write to private field"), e.set(t, o), o), R = (t, e, o) => (rc(t, e, "access private method"), o), xe, it, H, Lo, pe, et, ga, us, nm, Mr, ac, ds, lm, ms, cm, ps, um, _a, sc, va, nc, hs, dm, Wt, To, Vt, Mo;
const a_ = "form-configure-workflow-stage";
let fe = class extends be {
  constructor() {
    super(), j(this, us), j(this, Mr), j(this, ds), j(this, ms), j(this, ps), j(this, _a), j(this, va), j(this, hs), j(this, Wt), j(this, Vt), j(this, xe, void 0), j(this, it, void 0), this.collectionName = "", j(this, H, []), j(this, Lo, {}), j(this, pe, void 0), this.label = "", this.description = "", this.icon = "", this.allFields = [], j(this, et, []), j(this, ga, void 0), this.consumeContext(D, (t) => {
      Xe(this, xe, t);
    }), this.consumeContext(Ae, (t) => {
      Xe(this, it, t);
    });
  }
  set workflows(t) {
    Xe(this, H, structuredClone(t));
    for (let e = 0; e < v(this, H).length; e++) {
      const o = v(this, H)[e];
      v(this, Lo)[o.id] = this.collectionName;
    }
    R(this, us, nm).call(this), R(this, Mr, ac).call(this);
  }
  get workflows() {
    return v(this, H);
  }
  get workflowStages() {
    return v(this, Lo);
  }
  set submitMessageDetail(t) {
    Xe(this, pe, structuredClone(t));
  }
  get submitMessageDetail() {
    return v(this, pe);
  }
  render() {
    return n` ${R(this, Vt, Mo).call(this, {
      icon: this.icon,
      label: this.label,
      description: this.description
    })}
      ${m(
      this.submitMessageDetail,
      () => n`<uui-button @click=${R(this, ds, lm)}>
          ${R(this, Vt, Mo).call(this, {
        icon: "icon-document",
        label: this.localize.term("formWorkflows_submitMessage"),
        description: this.localize.term(
          "formWorkflows_defaultWorkflowDescription"
        ),
        iconClass: "square"
      })}
        </uui-button>`
    )}
      <div class="workflows-${this.collectionName}">
        ${this.workflows.map(
      (t, e) => n` <div
              class="sortable-stage workflow-${this.collectionName}"
              sort-unique=${t.id}
            >
              <uui-button
                @click=${async () => await R(this, ms, cm).call(this, e)}
              >
                ${R(this, Vt, Mo).call(this, {
        icon: t.workflowTypeIcon,
        label: t.name,
        description: t.workflowTypeDescription,
        iconClass: "square"
      })}
              </uui-button>
              ${m(
        !t.isMandatory,
        () => n`
                  <uui-action-bar>
                    <uui-button
                      label=${this.localize.term("general_remove")}
                      @click=${() => R(this, hs, dm).call(this, e)}
                    ></uui-button>
                  </uui-action-bar>`
      )}
            </div>`
    )}
      </div>
      <uui-button @click=${R(this, ps, um)}>
        ${R(this, Vt, Mo).call(this, {
      label: this.localize.term("formWorkflows_addWorkflow"),
      iconClass: "square dashed"
    })}
      </uui-button>`;
  }
};
xe = /* @__PURE__ */ new WeakMap();
it = /* @__PURE__ */ new WeakMap();
H = /* @__PURE__ */ new WeakMap();
Lo = /* @__PURE__ */ new WeakMap();
pe = /* @__PURE__ */ new WeakMap();
et = /* @__PURE__ */ new WeakMap();
ga = /* @__PURE__ */ new WeakMap();
us = /* @__PURE__ */ new WeakSet();
nm = function() {
  Xe(this, ga, new ji(this, {
    ...new Yi(
      "Forms.SorterIdentifier.Workflows",
      `.workflow-${this.collectionName}`,
      `.workflows-${this.collectionName}`
    ).config,
    onChange: ({ model: t }) => {
      Xe(this, et, t);
    },
    onEnd: () => {
      Xe(this, H, [...v(this, H)].sort(
        (t, e) => v(this, et).indexOf(t.id) - v(this, et).indexOf(e.id)
      )), R(this, Wt, To).call(this);
    }
  }));
};
Mr = /* @__PURE__ */ new WeakSet();
ac = function() {
  var t;
  Xe(this, et, []);
  for (let e = 0; e < v(this, H).length; e++) {
    const o = v(this, H)[e];
    v(this, et).push(o.id);
  }
  (t = v(this, ga)) == null || t.setModel(v(this, et));
};
ds = /* @__PURE__ */ new WeakSet();
lm = async function() {
  var r, i, a;
  if (!v(this, xe) || !v(this, it))
    return;
  const t = await v(this, it).getRichTextConfiguration(), o = await v(this, xe).open(
    this,
    Jd,
    {
      data: {
        richTextConfiguration: t
      },
      value: {
        messageOnSubmit: (r = v(this, pe)) == null ? void 0 : r.messageOnSubmit,
        messageOnSubmitIsHtml: ((i = v(this, pe)) == null ? void 0 : i.messageOnSubmitIsHtml) || !1,
        goToPageOnSubmit: (a = v(this, pe)) == null ? void 0 : a.goToPageOnSubmit
      }
    }
  ).onSubmit().catch(() => {
  });
  !o || !v(this, pe) || (v(this, pe).messageOnSubmit = o.messageOnSubmit, v(this, pe).messageOnSubmitIsHtml = o.messageOnSubmitIsHtml, v(this, pe).goToPageOnSubmit = o.goToPageOnSubmit, R(this, Wt, To).call(this));
};
ms = /* @__PURE__ */ new WeakSet();
cm = async function(t) {
  if (!v(this, xe))
    return;
  const e = v(this, H)[t], r = await (await R(this, _a, sc).call(this, e, !1)).onSubmit().catch(() => {
  });
  r && (R(this, va, nc).call(this, v(this, H)[t], r), v(this, Lo)[v(this, H)[t].id] = r.collectionName, R(this, Wt, To).call(this));
};
ps = /* @__PURE__ */ new WeakSet();
um = async function() {
  var a;
  if (!v(this, xe))
    return;
  if (!v(this, it))
    throw new Error("No workspace context");
  const e = await v(this, xe).open(
    this,
    Og
  ).onSubmit().catch(() => {
  });
  if (!(e != null && e.selectedValue))
    return;
  const o = ot.getWorkflowScaffold();
  o.form = (a = v(this, it)) == null ? void 0 : a.getUnique(), o.sortOrder = v(this, H).length, o.workflowTypeId = e.selectedValue.id, o.workflowTypeName = e.selectedValue.name, o.workflowTypeIcon = e.selectedValue.icon, o.workflowTypeDescription = e.selectedValue.description, o.workflowTypeGroup = e.selectedValue.group;
  const i = await (await R(this, _a, sc).call(this, o, !0)).onSubmit().catch(() => {
  });
  i && (R(this, va, nc).call(this, o, i), v(this, H).push(o), R(this, Mr, ac).call(this), R(this, Wt, To).call(this));
};
_a = /* @__PURE__ */ new WeakSet();
sc = async function(t, e) {
  const o = await v(this, it).loadWorkflowType(
    t.workflowTypeId
  );
  if (!o)
    throw new Error(
      "Workflow type with id " + t.workflowTypeId + " could not be found."
    );
  const r = t.settings;
  if (e)
    for (let i = 0; i < o.settings.length; i++) {
      const a = o.settings[i];
      a.defaultValue && (r[a.alias] = a.defaultValue);
    }
  return v(this, xe).open(this, em, {
    data: {
      fields: this.allFields,
      workflowType: o,
      isNew: e
    },
    value: {
      name: t.name,
      active: t.active,
      includeSensitiveData: t.includeSensitiveData === Je.TRUE,
      collectionName: this.collectionName,
      settings: r,
      condition: t.condition
    }
  });
};
va = /* @__PURE__ */ new WeakSet();
nc = function(t, e) {
  t.name = e.name, t.active = e.active, t.includeSensitiveData = e.includeSensitiveData ? Je.TRUE : Je.FALSE, t.settings = e.settings, t.condition = e.condition;
};
hs = /* @__PURE__ */ new WeakSet();
dm = function(t) {
  v(this, H).splice(t, 1), R(this, Wt, To).call(this);
};
Wt = /* @__PURE__ */ new WeakSet();
To = function() {
  this.requestUpdate(), this.dispatchEvent(
    new CustomEvent("change", { composed: !0, bubbles: !0 })
  );
};
Vt = /* @__PURE__ */ new WeakSet();
Mo = function(t) {
  return n`<div class="stage-block">
      <uui-icon
        .name=${t.icon ?? null}
        class="stage-icon ${t.iconClass}"
      ></uui-icon>
      <div>
        ${m(t.label, () => n`<strong>${t.label}</strong>`)}
        ${m(
    t.description,
    () => n`<small> ${t.description} </small>`
  )}
      </div>
    </div>`;
};
fe.styles = [
  C`
      :host {
        display: block;
        position: relative;
        z-index: 1;
      }

      :host:after {
        content: "";
        display: block;
        width: 1px;
        background-color: var(--uui-color-border-standalone);
        position: absolute;
        top: var(--uui-size-5);
        bottom: var(--uui-size-5);
        left: 27px;
        z-index: -1;
      }

      uui-button {
        --uui-button-padding-left-factor: 1;
        --uui-button-padding-right-factor: 1;
        --uui-button-padding-bottom-factor: 1;
        --uui-button-padding-top-factor: 1;
        text-align: left;
      }

      /* uui-button internally multiplies the above factors by --uui-size 2,
        but since we're setting the factor to 1, we don't need the calc()
      */
      :host > .stage-block {
        padding: var(--uui-size-2);
      }

      .stage-block {
        display: flex;
        column-gap: var(--uui-size-3);
        align-items: center;
        --icon-radius: 50%;
        --border-style: solid;
      }

      .stage-block > div {
        display: flex;
        flex-direction: column;
        line-height: 1.4;
      }

      .stage-icon {
        position: relative;
        background: white;
        padding: var(--uui-size-3);
        border-radius: var(--icon-radius);
        border-width: 1px;
        border-style: var(--border-style);
        border-color: var(--uui-color-border-standalone);
        width: 22px;
        height: 22px;
      }

      .square {
        --icon-radius: var(--uui-border-radius);
      }

      .dashed {
        --border-style: dashed;
      }

      .sortable-stage {
        display: flex;
        align-items: center;
        cursor: move;
      }

      .sortable-stage > uui-button {
        margin-right: auto;
      }

      uui-action-bar {
        opacity: 0;
        transition: opacity 120ms;
        margin-left: auto;
      }

      .sortable-stage:hover uui-action-bar {
        opacity: 1;
      }
    `
];
Re([
  p()
], fe.prototype, "collectionName", 2);
Re([
  p({ type: Array })
], fe.prototype, "workflows", 1);
Re([
  p()
], fe.prototype, "workflowStages", 1);
Re([
  p({ type: Object })
], fe.prototype, "submitMessageDetail", 1);
Re([
  p()
], fe.prototype, "label", 2);
Re([
  p()
], fe.prototype, "description", 2);
Re([
  p()
], fe.prototype, "icon", 2);
Re([
  p({ type: Array })
], fe.prototype, "allFields", 2);
fe = Re([
  h(a_)
], fe);
var s_ = Object.defineProperty, n_ = Object.getOwnPropertyDescriptor, l_ = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? n_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && s_(e, o, i), i;
}, c_ = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, zu = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, ur = (t, e, o) => (c_(t, e, "access private method"), o), Ao, dr, lc, mm;
const u_ = "form-configure-workflow-modal";
let jo = class extends ge {
  constructor() {
    super(...arguments), zu(this, Ao), zu(this, lc);
  }
  render() {
    var t, e, o, r, i;
    return n`<umb-body-layout headline=${this.localize.term("formWorkflows_configureWorkflow")}>
      <uui-box>
        <form-configure-workflow-stage
          collectionName="onSubmit"
          .workflows=${((t = this.value.workflows) == null ? void 0 : t.onSubmit) ?? []}
          .submitMessageDetail=${this.value.submitMessageDetail}
          .allFields=${((e = this.data) == null ? void 0 : e.fields) ?? []}
          .label=${this.localize.term("formWorkflows_onSubmit")}
          .description=${this.localize.term(
      "formWorkflows_onSubmitDescription"
    )}
          icon="icon-check"
          @change=${(a) => ur(this, Ao, dr).call(this, a, "onSubmit")}
        >
        </form-configure-workflow-stage>
        <form-configure-workflow-stage
          collectionName="onApprove"
          .workflows=${((o = this.value.workflows) == null ? void 0 : o.onApprove) ?? []}
          .allFields=${((r = this.data) == null ? void 0 : r.fields) ?? []}
          .label=${this.localize.term("formWorkflows_onApprove")}
          .description=${this.localize.term(
      "formWorkflows_onApproveDescription"
    )}
          icon="icon-thumb-up"
          @change=${(a) => ur(this, Ao, dr).call(this, a, "onApprove")}
        >
        </form-configure-workflow-stage>

        ${m(
      (i = this.data) == null ? void 0 : i.manualApproval,
      () => {
        var a, s;
        return n`<form-configure-workflow-stage
            collectionName="onReject"
            .workflows=${((a = this.value.workflows) == null ? void 0 : a.onReject) ?? []}
            .allFields=${((s = this.data) == null ? void 0 : s.fields) ?? []}
            .label=${this.localize.term("formWorkflows_onReject")}
            .description=${this.localize.term(
          "formWorkflows_onRejectDescription"
        )}
            icon="icon-delete"
            @change=${(u) => ur(this, Ao, dr).call(this, u, "onReject")}
          >
          </form-configure-workflow-stage>`;
      }
    )}
      </uui-box>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click="${this._rejectModal}"
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          @click=${this._submitModal}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
Ao = /* @__PURE__ */ new WeakSet();
dr = function(t, e) {
  var u, S;
  const o = t.target, r = o.submitMessageDetail;
  r && ((u = this.modalContext) == null || u.updateValue({ submitMessageDetail: r }));
  const i = o.workflows, a = structuredClone(this.value.workflows);
  a && (a[e] = i, (S = this.modalContext) == null || S.updateValue({ workflows: a }));
  const s = o.workflowStages;
  for (let g = 0; g < i.length; g++) {
    const b = i[g], I = s[b.id];
    I && I !== e && ur(this, lc, mm).call(this, g, e, s[b.id]);
  }
};
lc = /* @__PURE__ */ new WeakSet();
mm = function(t, e, o) {
  var u;
  if (!this.value.workflows)
    return;
  const r = structuredClone(this.value.workflows), a = r[e].splice(t, 1)[0];
  r[o].push(a), (u = this.modalContext) == null || u.updateValue({ workflows: r });
};
jo.styles = C`
    form-configure-workflow-stage + form-configure-workflow-stage {
      margin-top: var(--uui-size-5);
    }
  `;
jo = l_([
  h(u_)
], jo);
const d_ = jo, m_ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsConfigureWorkflowModalElement() {
    return jo;
  },
  default: d_
}, Symbol.toStringTag, { value: "Module" })), Dt = "00000000-0000-0000-0000-000000000000", IP = {
  year: "numeric",
  month: "long",
  day: "numeric"
}, p_ = {
  year: "numeric",
  month: "long",
  day: "numeric",
  hour: "numeric",
  minute: "numeric",
  second: "numeric"
};
var h_ = Object.defineProperty, f_ = Object.getOwnPropertyDescriptor, y_ = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? f_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && h_(e, o, i), i;
}, cc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Ja = (t, e, o) => (cc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), We = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, g_ = (t, e, o, r) => (cc(t, e, "write to private field"), e.set(t, o), o), Pe = (t, e, o) => (cc(t, e, "access private method"), o), jt, fs, pm, ys, hm, gs, fm, _s, ym, Ki, Sa, vs, gm, uc, _m;
const __ = "form-create-from-datasource-modal";
let Go = class extends ge {
  constructor() {
    super(), We(this, fs), We(this, ys), We(this, gs), We(this, _s), We(this, Ki), We(this, vs), We(this, uc), We(this, jt, void 0), this.consumeContext(Fo, (t) => {
      g_(this, jt, t);
    });
  }
  render() {
    var t, e, o, r;
    return n`<umb-body-layout headline=${this.localize.term("formDataSources_createFormFromDataSource")}>
      <uui-box>
        <umb-property-layout
          alias="name"
          label=${this.localize.term("formDataSources_formName")}
        >
          <uui-input
            slot="editor"
            id="name"
            .value=${(e = (t = this.value) == null ? void 0 : t.wizard) == null ? void 0 : e.formName}
            @change=${Pe(this, fs, pm)}
            label="name"
          >
        </umb-property-layout>
        <umb-property-layout
          alias="mapping"
          label=${this.localize.term("formDataSources_selectFields")}
          description=${this.localize.term(
      "formDataSources_selectFieldsDescription"
    )}
        >
          <table
            slot="editor"
            >
            <thead>
              <tr>
                <th>${this.localize.term("general_name")}</th>
                <th>Include</th>
                <th>Field Type</th>
                <th>${this.localize.term("formDataSources_defaultValue")}</th>
              </tr>
            </thead>
            <tbody>
              ${Gf(
      (r = (o = this.value) == null ? void 0 : o.wizard) == null ? void 0 : r.mappings,
      (i, a) => {
        var s;
        return n`
                  <tr>
                    <td>${i.name}</td>
                    <td>
                      <uui-toggle
                        ?checked=${i.include}
                        @change=${(u) => Pe(this, ys, hm).call(this, u, a)}
                      ></uui-toggle>
                    </td>
                    <td>
                      <uui-select
                        .disabled=${!i.include}
                        @change=${(u) => Pe(this, gs, fm).call(this, u, a)}
                        .options=${((s = this.data) == null ? void 0 : s.fieldTypes.map((u, S) => ({
          name: u.name,
          value: u.id,
          selected: u.id === i.fieldTypeId
        }))) ?? []}
                      >
                      </uui-select>
                    </td>
                    <td>
                      <uui-input
                        id="defaultValue"
                        .value=${i.defaultValue}
                        @change=${(u) => Pe(this, _s, ym).call(this, u, a)}
                        label=${this.localize.term(
          "formDataSources_defaultValue"
        )}
                      >
                    </td>
                  </tr>
                `;
      }
    )}
            </tbody>
          </table>
        </umb-property-layout>
      </uui-box>
      <div slot="actions">
        <uui-button
          .label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          @click=${Pe(this, vs, gm)}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
jt = /* @__PURE__ */ new WeakMap();
fs = /* @__PURE__ */ new WeakSet();
pm = function(t) {
  var r, i, a;
  if (!((r = this.value) != null && r.wizard))
    return;
  const e = t.target.value.toString(), o = structuredClone((i = this.value) == null ? void 0 : i.wizard);
  o.formName = e, (a = this.modalContext) == null || a.updateValue({ wizard: o });
};
ys = /* @__PURE__ */ new WeakSet();
hm = function(t, e) {
  Pe(this, Ki, Sa).call(this, e, "include", t.target.checked);
};
gs = /* @__PURE__ */ new WeakSet();
fm = function(t, e) {
  Pe(this, Ki, Sa).call(this, e, "fieldTypeId", t.target.value.toString());
};
_s = /* @__PURE__ */ new WeakSet();
ym = function(t, e) {
  Pe(this, Ki, Sa).call(this, e, "defaultValue", t.target.value.toString());
};
Ki = /* @__PURE__ */ new WeakSet();
Sa = function(t, e, o) {
  var i, a, s;
  if (!((i = this.value) != null && i.wizard))
    return;
  const r = structuredClone((a = this.value) == null ? void 0 : a.wizard);
  r.mappings[t][e] = o, (s = this.modalContext) == null || s.updateValue({ wizard: r });
};
vs = /* @__PURE__ */ new WeakSet();
gm = async function() {
  var t, e, o, r, i;
  if ((t = this.value) != null && t.wizard)
    if (Pe(this, uc, _m).call(this)) {
      const { error: a } = await lo(
        Ke.postDatasourceWizardCreateForm({
          requestBody: this.value.wizard
        })
      );
      if (a) {
        const s = { data: { message: "Could not create form." } };
        (r = Ja(this, jt)) == null || r.peek("danger", s);
      } else {
        const s = {
          data: {
            message: "Form '" + this.value.wizard.formName + "' created."
          }
        };
        (e = Ja(this, jt)) == null || e.peek("positive", s), (o = this.modalContext) == null || o.submit();
      }
    } else {
      const a = {
        data: {
          message: "Could not create form. Please select a type for each included field."
        }
      };
      (i = Ja(this, jt)) == null || i.peek("danger", a);
    }
};
uc = /* @__PURE__ */ new WeakSet();
_m = function() {
  var t;
  if (!((t = this.value) != null && t.wizard))
    return !1;
  for (let e = 0; e < this.value.wizard.mappings.length; e++) {
    const o = this.value.wizard.mappings[e];
    if (o.include && o.fieldTypeId === Dt)
      return !1;
  }
  return !0;
};
Go.styles = [C``];
Go = y_([
  h(__)
], Go);
const v_ = Go, S_ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFormCreateFromDataSourceModalElement() {
    return Go;
  },
  default: v_
}, Symbol.toStringTag, { value: "Module" }));
var w_ = Object.defineProperty, b_ = Object.getOwnPropertyDescriptor, wa = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? b_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && w_(e, o, i), i;
}, dc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, z = (t, e, o) => (dc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), Z = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Nu = (t, e, o, r) => (dc(t, e, "write to private field"), e.set(t, o), o), V = (t, e, o) => (dc(t, e, "access private method"), o), Bt, N, mc, vm, Ss, Sm, ws, wm, bs, bm, pc, Fm, hc, Em, Ar, Fs, fc, Tm, Ie, nt, Es, $m, Ts, Cm;
const F_ = "form-edit-conditions";
var Xi = /* @__PURE__ */ ((t) => (t[t.FIELD = 0] = "FIELD", t[t.FIELDSET = 1] = "FIELDSET", t[t.PAGE = 2] = "PAGE", t[t.WORKFLOW = 3] = "WORKFLOW", t))(Xi || {});
let uo = class extends be {
  constructor() {
    super(), Z(this, mc), Z(this, Ss), Z(this, ws), Z(this, bs), Z(this, pc), Z(this, hc), Z(this, Ar), Z(this, fc), Z(this, Ie), Z(this, Es), Z(this, Ts), Z(this, Bt, void 0), Z(this, N, {
      id: "",
      enabled: !1,
      actionType: Ql.SHOW,
      logicType: Jl.ALL,
      rules: []
    }), this.fields = [], this.appliedTo = 0, this.consumeContext(Ae, (t) => {
      Nu(this, Bt, t);
    });
  }
  set value(t) {
    Nu(this, N, t ? structuredClone(t) : z(this, N));
    for (let e = 0; e < z(this, N).rules.length; e++)
      z(this, N).rules[e] = structuredClone(z(this, N).rules[e]);
  }
  get value() {
    return z(this, N);
  }
  render() {
    var t, e;
    return n` <div class="flex center">
        <uui-select
          name="actionType"
          @change=${V(this, Ss, Sm)}
          .options=${((t = z(this, Bt)) == null ? void 0 : t.getConditionActionTypes.map(
      (o) => ({
        name: this.localize.term(`formConditions_${this.appliedTo === 3 ? "workflow" : ""}actionType${o.value}`),
        value: o.value,
        selected: o.value === z(this, N).actionType
      })
    )) ?? []}
        ></uui-select>
        <span>${this.localize.term("formConditions_" + V(this, Es, $m).call(this))}</span>
        <uui-select
          name="logicType"
          @change=${V(this, ws, wm)}
          .options=${((e = z(this, Bt)) == null ? void 0 : e.getConditionLogicTypes.map(
      (o) => ({
        name: this.localize.term("formConditions_logicType" + o.value),
        value: o.value,
        selected: o.value === z(this, N).logicType
      })
    )) ?? []}
        >
        </uui-select>
        <span
          >${this.localize.term("formConditions_ofTheFollowingMatch")}:</span
        >
      </div>

      ${z(this, N).rules.map((o, r) => V(this, Ts, Cm).call(this, o, r))}

      <uui-button
        label=${this.localize.term("formConditions_addCondition")}
        look="secondary"
        color="default"
        @click=${V(this, bs, bm)}
        >${this.localize.term("formConditions_addCondition")}</uui-button
      >`;
  }
};
Bt = /* @__PURE__ */ new WeakMap();
N = /* @__PURE__ */ new WeakMap();
mc = /* @__PURE__ */ new WeakSet();
vm = function(t) {
  const e = this.fields.find((o) => o.id === t);
  return (e == null ? void 0 : e.preValues.map((o) => o.value)) ?? [];
};
Ss = /* @__PURE__ */ new WeakSet();
Sm = function(t) {
  z(this, N).actionType = t.target.value, V(this, Ie, nt).call(this);
};
ws = /* @__PURE__ */ new WeakSet();
wm = function(t) {
  z(this, N).logicType = t.target.value, V(this, Ie, nt).call(this);
};
bs = /* @__PURE__ */ new WeakSet();
bm = function() {
  z(this, N).rules.push({
    id: q.new(),
    field: Dt,
    operator: Pd.IS,
    value: ""
  }), V(this, Ie, nt).call(this);
};
pc = /* @__PURE__ */ new WeakSet();
Fm = function(t, e) {
  z(this, N).rules[e].field = t.target.value.toString(), V(this, Ie, nt).call(this);
};
hc = /* @__PURE__ */ new WeakSet();
Em = function(t, e) {
  z(this, N).rules[e].operator = t.target.value, V(this, Ie, nt).call(this);
};
Ar = /* @__PURE__ */ new WeakSet();
Fs = function(t, e) {
  z(this, N).rules[e].value = t, V(this, Ie, nt).call(this);
};
fc = /* @__PURE__ */ new WeakSet();
Tm = function(t) {
  z(this, N).rules.splice(t, 1), V(this, Ie, nt).call(this);
};
Ie = /* @__PURE__ */ new WeakSet();
nt = function() {
  this.requestUpdate(), this.dispatchEvent(
    new CustomEvent("change", { composed: !0, bubbles: !0 })
  );
};
Es = /* @__PURE__ */ new WeakSet();
$m = function() {
  switch (this.appliedTo) {
    case 1:
      return "thisFieldSetIf";
    case 2:
      return "buttonsForThisPageIf";
    case 3:
      return "thisWorkflowIf";
    default:
      return "thisFieldIf";
  }
};
Ts = /* @__PURE__ */ new WeakSet();
Cm = function(t, e) {
  var r;
  const o = V(this, mc, vm).call(this, t.field);
  return n`<div class="flex center condition-rule">
      <uui-select
        name="field"
        .placeholder=${this.localize.term("formConditions_selectField")}
        @change=${(i) => V(this, pc, Fm).call(this, i, e)}
        .options=${this.fields.map((i) => ({
    name: i.caption,
    value: i.id,
    selected: i.id === t.field
  }))}
      >
      </uui-select>

      <uui-select
        name="operator"
        @change=${(i) => V(this, hc, Em).call(this, i, e)}
        .options=${((r = z(this, Bt)) == null ? void 0 : r.getConditionOperators.map((i) => ({
    name: this.localize.term("formConditions_operator" + i.value),
    value: i.value,
    selected: i.value === t.operator
  }))) ?? []}
      >
      </uui-select>

      ${m(
    o.length > 0,
    () => n`<uui-select
          name="value"
          @change=${(i) => V(this, Ar, Fs).call(this, i.target.value.toString(), e)}
          .options=${o.map((i) => ({
      name: i,
      value: i,
      selected: i === t.value
    }))}
        >
        </uui-select>`,
    () => n`<uui-input
          name="value"
          type="text"
          value=${t.value}
          @change=${(i) => V(this, Ar, Fs).call(this, i.target.value.toString(), e)}
        ></uui-input>`
  )}

      <uui-button
        label=${this.localize.term("general_delete")}
        look="secondary"
        color="default"
        @click=${() => V(this, fc, Tm).call(this, e)}
      >
        <uui-icon name="delete"></uui-icon>
      </uui-button>
    </div>`;
};
uo.styles = [
  C`
      .flex {
        display: flex;
        column-gap: var(--uui-size-3);
      }
      .center {
        align-items: center;
      }

      :host > uui-button {
        margin-top: var(--uui-size-5);
      }

      .condition-rule {
        margin-top: var(--uui-size-3);
      }

      .condition-rule:first-of-type {
        margin-top: var(--uui-size-5);
      }
    `
];
wa([
  p({ type: Object })
], uo.prototype, "value", 1);
wa([
  p({ type: Array })
], uo.prototype, "fields", 2);
wa([
  p({ type: Xi })
], uo.prototype, "appliedTo", 2);
uo = wa([
  h(F_)
], uo);
var E_ = Object.defineProperty, T_ = Object.getOwnPropertyDescriptor, Om = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? T_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && E_(e, o, i), i;
}, $_ = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, qu = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Vu = (t, e, o) => ($_(t, e, "access private method"), o), $s, Pm, Cs, km;
const C_ = "form-edit-page-modal";
let Ho = class extends ge {
  constructor() {
    super(...arguments), qu(this, $s), qu(this, Cs), this._conditionEnabled = !1;
  }
  connectedCallback() {
    var t, e;
    super.connectedCallback(), this._conditionEnabled = ((e = (t = this.value) == null ? void 0 : t.buttonCondition) == null ? void 0 : e.enabled) ?? !1;
  }
  render() {
    return n`<umb-body-layout headline=${this.localize.term("formEdit_editPage")}>
      <div id="main">
        <uui-box>
          <umb-property-layout
            alias="conditions"
            .label=${this.localize.term("formConditions_title")}
            .description=${this.localize.term(
      "formConditions_pageConditionsDescription"
    )}
          >
            <div slot="editor">
              <uui-toggle
                ?checked=${this._conditionEnabled}
                .label=${this._conditionEnabled ? "On" : "Off"}
                @change=${Vu(this, $s, Pm)}
              ></uui-toggle>
              ${m(
      this._conditionEnabled,
      () => {
        var t;
        return n`<form-edit-conditions
                    .value=${this.value.buttonCondition}
                    .fields=${((t = this.data) == null ? void 0 : t.fields) ?? []}
                    .appliedTo=${Xi.PAGE}
                    @change=${Vu(this, Cs, km)}
                  ></form-edit-conditions>`;
      }
    )}
            </div>
          </umb-property-layout>
        </uui-box>
      </div>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          @click=${this._submitModal}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
$s = /* @__PURE__ */ new WeakSet();
Pm = function(t) {
  var o;
  this._conditionEnabled = t.target.checked;
  const e = {
    ...this.value.buttonCondition,
    enabled: this._conditionEnabled
  };
  (o = this.modalContext) == null || o.updateValue({ buttonCondition: e });
};
Cs = /* @__PURE__ */ new WeakSet();
km = function(t) {
  var o;
  const e = t.target.value ?? void 0;
  (o = this.modalContext) == null || o.updateValue({ buttonCondition: e });
};
Om([
  w()
], Ho.prototype, "_conditionEnabled", 2);
Ho = Om([
  h(C_)
], Ho);
const O_ = Ho, P_ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsEditPageModalElement() {
    return Ho;
  },
  default: O_
}, Symbol.toStringTag, { value: "Module" })), Qi = new M("forms-context");
var Jt, Zt, sa, Dm;
class Bu extends Hf {
  constructor(o) {
    super(o, Qi);
    f(this, sa);
    f(this, Jt, void 0);
    f(this, Zt, void 0);
    _(this, Jt, new xu(void 0)), this.config = d(this, Jt).asObservable(), _(this, Zt, new xu(
      void 0
    )), this.userSecurity = d(this, Zt).asObservable();
  }
  async hostConnected() {
    super.hostConnected(), this.consumeContext(Fd, (o) => {
      this.observe(o.currentUser, async (r) => {
        var a;
        (((a = r == null ? void 0 : r.allowedSections) == null ? void 0 : a.includes("Umb.Section.Forms")) ?? !1) && (F(this, sa, Dm).call(this), this.getUserSecurity());
      });
    });
  }
  async getUserSecurity() {
    const { data: o } = await y(
      this,
      ie.getSecurityUserCurrentFormSecurity({
        includeFormFieldDetails: !1
      })
    );
    d(this, Zt).setValue(o);
  }
}
Jt = new WeakMap(), Zt = new WeakMap(), sa = new WeakSet(), Dm = async function() {
  const { data: o } = await y(this, vy.getConfig());
  d(this, Jt).setValue(o);
};
const k_ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  FormsContext: Bu,
  default: Bu
}, Symbol.toStringTag, { value: "Module" }));
var D_ = Object.defineProperty, x_ = Object.getOwnPropertyDescriptor, yc = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? x_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && D_(e, o, i), i;
}, gc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, xm = (t, e, o) => (gc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), $e = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, M_ = (t, e, o, r) => (gc(t, e, "write to private field"), e.set(t, o), o), ze = (t, e, o) => (gc(t, e, "access private method"), o), Rr, Os, Mm, Ps, Am, ks, Rm, ba, _c, Ds, Im, xs, Um, Ms, Wm, As, Lm;
const A_ = "form-edit-fieldset-modal";
let xt = class extends ge {
  constructor() {
    super(), $e(this, Os), $e(this, Ps), $e(this, ks), $e(this, ba), $e(this, Ds), $e(this, xs), $e(this, Ms), $e(this, As), this._conditionEnabled = !1, this._containers = [], $e(this, Rr, 12), this.consumeContext(Qi, (t) => {
      t && this.observe(t.config, (e) => {
        e && M_(this, Rr, e == null ? void 0 : e.maxNumberOfColumnsInFormGroup);
      });
    });
  }
  connectedCallback() {
    var t, e, o;
    super.connectedCallback(), this._conditionEnabled = ((e = (t = this.value) == null ? void 0 : t.condition) == null ? void 0 : e.enabled) ?? !1, this._containers = structuredClone((o = this.value) == null ? void 0 : o.containers) ?? [];
  }
  render() {
    var t;
    return n`<umb-body-layout headline=${this.localize.term("formEdit_editGroup")}>
      <div id="main">
        <uui-box>
          <umb-property-layout alias="page" label="Page">
            <div slot="editor">
              <uui-select
                label="Page"
                @change=${ze(this, Os, Mm)}
                .options=${((t = this.data) == null ? void 0 : t.pages.map((e, o) => {
      var r;
      return {
        name: e.caption ?? `Page ${o + 1}`,
        value: e.id,
        selected: o === ((r = this.value) == null ? void 0 : r.pageIndex)
      };
    })) ?? []}
              >
              </uui-select>
            </div>
          </umb-property-layout>

          <umb-property-layout
            alias="columns"
            orientation="vertical"
            .label=${this.localize.term("fieldSetColumns_title")}
            .description=${this.localize.term("fieldSetColumns_setNumber")}
          >
            <div slot="editor">
              <strong
                >${this.localize.term(
      "fieldSetColumns_columnNumberDescription",
      this._containers.length
    )}</strong
              >
              <div id="columnsTemplate">
                ${this._containers.map(
      (e, o) => m(
        this._containers.length === 1,
        () => n`<div></div>`,
        () => n`<uui-button
                      id="container-${o}"
                      label="Remove column"
                      color="default"
                      look="primary"
                      @click=${() => ze(this, ks, Rm).call(this, o)}
                    ></uui-button>`
      )
    )}
              </div>
              <uui-button
                id="addColumn"
                label="Add column"
                look="primary"
                color="default"
                ?disabled=${xm(this, As, Lm)}
                @click=${ze(this, Ps, Am)}
              ></uui-button>
            </div>
          </umb-property-layout>
          <umb-property-layout
            alias="conditions"
            .label=${this.localize.term("formConditions_title")}
          >
            <div slot="editor">
              <uui-toggle
                ?checked=${this._conditionEnabled}
                .label=${this._conditionEnabled ? "On" : "Off"}
                @change=${ze(this, Ds, Im)}
              ></uui-toggle>
              ${m(
      this._conditionEnabled,
      () => {
        var e;
        return n`<form-edit-conditions
                    .value=${this.value.condition}
                    .fields=${((e = this.data) == null ? void 0 : e.fields) ?? []}
                    .appliedTo=${Xi.FIELDSET}
                    @change=${ze(this, xs, Um)}
                  ></form-edit-conditions>`;
      }
    )}
            </div>
          </umb-property-layout>
        </uui-box>
      </div>
      <div slot="actions">
        <uui-button
          .label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          @click=${ze(this, Ms, Wm)}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
Rr = /* @__PURE__ */ new WeakMap();
Os = /* @__PURE__ */ new WeakSet();
Mm = function(t) {
  var o;
  const e = t.target._input.selectedIndex - 1;
  (o = this.modalContext) == null || o.updateValue({ pageIndex: e });
};
Ps = /* @__PURE__ */ new WeakSet();
Am = function() {
  this._containers.push(ot.getContainerScaffold()), ze(this, ba, _c).call(this);
};
ks = /* @__PURE__ */ new WeakSet();
Rm = function(t) {
  this._containers.splice(t, 1), ze(this, ba, _c).call(this);
};
ba = /* @__PURE__ */ new WeakSet();
_c = function() {
  const t = 12 / this._containers.length;
  this._containers.forEach((e) => e.width = t), this.requestUpdate();
};
Ds = /* @__PURE__ */ new WeakSet();
Im = function(t) {
  var o;
  this._conditionEnabled = t.target.checked;
  const e = structuredClone(this.value.condition);
  e.enabled = this._conditionEnabled, (o = this.modalContext) == null || o.updateValue({ condition: e });
};
xs = /* @__PURE__ */ new WeakSet();
Um = function(t) {
  var o;
  const e = t.target.value ?? void 0;
  (o = this.modalContext) == null || o.updateValue({ condition: e });
};
Ms = /* @__PURE__ */ new WeakSet();
Wm = function() {
  var t, e;
  (t = this.modalContext) == null || t.updateValue({ containers: this._containers }), (e = this.modalContext) == null || e.submit();
};
As = /* @__PURE__ */ new WeakSet();
Lm = function() {
  return this._containers.length === xm(this, Rr);
};
xt.styles = [
  C`
      #columnsTemplate {
        width: 100%;
        height: var(--uui-size-24);
        box-sizing: border-box;
        padding: var(--uui-size-1);
        border: 2px solid var(--uui-color-border);
        display: flex;
        flex-direction: row;
        flex-wrap: nowrap;
        margin-bottom: var(--uui-size-4);
        gap: var(--uui-size-1);
        border-radius: var(--uui-border-radius);
      }

      #columnsTemplate > * {
        background: var(--uui-color-interactive);
        flex: 1;
      }
    `
];
yc([
  w()
], xt.prototype, "_conditionEnabled", 2);
yc([
  w()
], xt.prototype, "_containers", 2);
xt = yc([
  h(A_)
], xt);
const R_ = xt, I_ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsEditFieldsetModalElement() {
    return xt;
  },
  default: R_
}, Symbol.toStringTag, { value: "Module" }));
var Fi, eo, wt, Ei, Ti, $i, Rs, to, mr, na, zm, oo, pr, la, Nm;
class U_ {
  constructor(e, o, r) {
    f(this, $i);
    f(this, to);
    f(this, na);
    f(this, oo);
    f(this, la);
    f(this, Fi, void 0);
    f(this, eo, void 0);
    f(this, wt, void 0);
    f(this, Ei, void 0);
    f(this, Ti, {});
    _(this, Fi, e), _(this, eo, new Qf(d(this, Fi))), _(this, wt, o), _(this, Ei, r);
  }
  async loadSettingValueConverterApis() {
    const e = Td.getAllExtensions().filter((o) => o.type === "formsSettingValueConverter").map((o) => o);
    for (let o = 0; o < e.length; o++) {
      const r = e[o], i = await Jf(r.api);
      if (i) {
        const a = new i();
        a && (d(this, Ti)[r.propertyEditorUiAlias] = a);
      }
    }
  }
  setProviderType(e) {
    _(this, wt, e);
  }
  getLocalizedSettingDetail(e, o, r) {
    const i = d(this, Ei) + "_" + d(this, wt).alias + e.alias + o, a = d(this, eo).term(i);
    return a && a.length > 0 && a !== i ? a : r;
  }
  async getSettingValues(e) {
    const o = [];
    for (const [r, i] of Object.entries(e)) {
      const a = await F(this, na, zm).call(this, r, i);
      o.push({ alias: r, value: a });
    }
    return o;
  }
  async getSettingsConfig(e, o) {
    const r = [];
    for (const i of e)
      await F(this, $i, Rs).call(this, r, i.alias, e);
    for (const i of o)
      r.find((a) => a.alias === i.alias) || await F(this, $i, Rs).call(this, r, i.alias, []);
    return r;
  }
  async getSettingPropertyConfig(e, o) {
    const r = F(this, oo, pr).call(this, e);
    if (!r)
      return [];
    const i = F(this, to, mr).call(this, r.view);
    return i ? await i.getSettingPropertyConfig(r, e, o) : [];
  }
  async getUpdatedSettingsForPersistence(e, o) {
    if (o) {
      const i = Object.fromEntries(o.map((a) => [a.alias, a.defaultValue]));
      e = e.filter((a) => a.value !== i[a.alias]);
    }
    const r = {};
    for (let i = 0; i < e.length; i++) {
      const a = e[i];
      r[a.alias] = await F(this, la, Nm).call(this, a);
    }
    return r;
  }
  createValidationErrorNotification(e, o) {
    const r = o.body.detail.split("|").join(", ");
    return {
      data: {
        headline: d(this, eo).term(e),
        message: r
      }
    };
  }
  getPropertyConfigForSetting(e, o) {
    var r;
    return ((r = e.find((i) => i.alias === o.alias)) == null ? void 0 : r.value) ?? [];
  }
  getPropertyAppearanceForSetting(e) {
    return e === "Umb.PropertyEditorUi.Tiptap" ? { labelOnTop: !0 } : void 0;
  }
}
Fi = new WeakMap(), eo = new WeakMap(), wt = new WeakMap(), Ei = new WeakMap(), Ti = new WeakMap(), $i = new WeakSet(), Rs = async function(e, o, r) {
  e.push({
    alias: o,
    value: await this.getSettingPropertyConfig(o, r)
  });
}, to = new WeakSet(), mr = function(e) {
  return d(this, Ti)[e];
}, na = new WeakSet(), zm = async function(e, o) {
  const r = F(this, oo, pr).call(this, e);
  if (!r)
    return null;
  const i = F(this, to, mr).call(this, r.view);
  return i ? await i.getSettingValueForEditor(r, e, o) : o;
}, oo = new WeakSet(), pr = function(e) {
  return d(this, wt).settings.find((o) => o.alias === e);
}, la = new WeakSet(), Nm = async function(e) {
  const o = F(this, oo, pr).call(this, e.alias);
  if (!o)
    return "";
  const r = F(this, to, mr).call(this, o.view);
  return r ? await r.getSettingValueForPersistence(o, e) : e.value;
};
var W_ = Object.defineProperty, L_ = Object.getOwnPropertyDescriptor, lt = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? L_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && W_(e, o, i), i;
}, vc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, le = (t, e, o) => (vc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), O = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, ju = (t, e, o, r) => (vc(t, e, "write to private field"), e.set(t, o), o), P = (t, e, o) => (vc(t, e, "access private method"), o), Q, hr, Is, qm, Us, Vm, Ws, Bm, Ls, jm, zs, Gm, Ns, Hm, qs, Ym, Vs, Km, Bs, Xm, js, Qm, Gs, Jm, Hs, Zm, Ys, ep, Ks, tp, Xs, op, Qs, ip, Js, rp, Zs, ap, en, sp, tn, np, Ir, Sc, Fa, wc, on, lp, rn, cp, an, up, sn, dp;
const z_ = "form-edit-field-modal";
let ue = class extends ge {
  constructor() {
    super(...arguments), O(this, Is), O(this, Us), O(this, Ws), O(this, Ls), O(this, zs), O(this, Ns), O(this, qs), O(this, Vs), O(this, Bs), O(this, js), O(this, Gs), O(this, Hs), O(this, Ys), O(this, Ks), O(this, Xs), O(this, Qs), O(this, Js), O(this, Zs), O(this, en), O(this, tn), O(this, Ir), O(this, Fa), O(this, on), O(this, rn), O(this, an), O(this, sn), O(this, Q, void 0), this._aliasLocked = !0, this._settingValues = [], this._settingsConfig = [], this._settingsConfigLoaded = !1, this._showRegex = !1, this._showRegexReadonly = !1, this._conditionEnabled = !1, O(this, hr, void 0);
  }
  async connectedCallback() {
    var e, o;
    super.connectedCallback();
    const t = await this.getContext(Fd);
    ju(this, hr, await Kl(t.currentUser)), ju(this, Q, new U_(
      this,
      this.value.fieldType,
      "formProviderFieldTypes"
    )), await le(this, Q).loadSettingValueConverterApis(), await P(this, Is, qm).call(this), this._settingsConfig = await le(this, Q).getSettingsConfig(
      this._settingValues,
      this.value.fieldType.settings
    ), this._settingsConfigLoaded = !0, this._conditionEnabled = ((o = (e = this.value) == null ? void 0 : e.condition) == null ? void 0 : o.enabled) ?? !1, P(this, Ir, Sc).call(this);
  }
  render() {
    var t, e, o, r, i, a, s, u, S, g, b, I, B, Y;
    return n`<umb-body-layout
      headline=${this.localize.term("formEdit_editField")}
    >
      <div id="main">
        <uui-box>
          <uui-input
            id="caption"
            .value=${(t = this.value) == null ? void 0 : t.caption}
            @change=${P(this, Us, Vm)}
            label="caption"
          >
            <uui-input
              name="alias"
              slot="append"
              label="alias"
              .value=${(e = this.value) == null ? void 0 : e.alias}
              @change=${P(this, Ls, jm)}
              placeholder="Enter alias..."
              ?disabled=${this._aliasLocked}
            >
              <!-- TODO: validation for bad characters -->
              <div
                @click=${P(this, Ws, Bm)}
                @keydown=${() => ""}
                id="alias-lock"
                slot="prepend"
              >
                <uui-icon
                  name=${this._aliasLocked ? "icon-lock" : "icon-unlocked"}
                ></uui-icon>
              </div> </uui-input
          ></uui-input>

          <umb-ref-property-editor-ui
            standalone
            .name=${(o = this.value) == null ? void 0 : o.fieldType.name}
            .alias=${(r = this.value) == null ? void 0 : r.fieldType.alias}
            .propertyEditorSchemaAlias=${(i = this.value) == null ? void 0 : i.fieldType.description}
          >
            <umb-icon name=${(a = this.value) == null ? void 0 : a.fieldType.icon} slot="icon"></umb-icon>
            <uui-action-bar slot="actions">
              <uui-button
                label="Change"
                @click=${P(this, Ns, Hm)}
              ></uui-button>
            </uui-action-bar>
          </umb-ref-property-editor-ui>

          <umb-property-layout
            alias="tooltip"
            label=${this.localize.term("formEdit_helpText")}
          >
            <uui-textarea
              slot="editor"
              name="tooltip"
              .value=${(s = this.value) == null ? void 0 : s.tooltip}
              @change=${P(this, zs, Gm)}
              label="tooltip"
            ></uui-textarea>
          </umb-property-layout>

          ${m(
      this.data && !this.data.isNew,
      () => n`<umb-property-layout alias="group" label="Group">
                <uui-select
                  label="Group"
                  slot="editor"
                  @change=${P(this, Vs, Km)}
                  .options=${P(this, qs, Ym).call(this).map((E) => ({
        name: E.name,
        value: E.id,
        selected: E.selected
      })) ?? []}
                >
                </uui-select>
              </umb-property-layout>`
    )}

          <umb-property-layout
              alias="fieldSettings_sensitiveData"
              .label=${this.localize.term("fieldSettings_sensitiveData")}
            >
            <!--
            // Verify that the current user is allowed to view & change the property 'containsSensitiveData'.
            // We allow a user that doesn't have the permission to set the value to true or false, but if
            // they or anyone else has previously set it to true, they aren't allowed to see or edit it.
            // See: https://github.com/umbraco/Umbraco.Forms.Issues/issues/1233
            -->
            ${(u = le(this, hr)) != null && u.hasAccessToSensitiveData || ((S = this.value) == null ? void 0 : S.containsSensitiveData) === !1 ? n`
                  <uui-toggle
                    slot="editor"
                    ?checked=${(g = this.value) == null ? void 0 : g.containsSensitiveData}
                    .label=${this.localize.term("fieldSettings_sensitiveDataLabel")}
                    @change=${P(this, Gs, Jm)}
                  ></uui-toggle>` : n`
                  <div slot="editor">${this.localize.term("fieldSettings_sensitiveDataLabel")}</div>`}
          </umb-property-layout>

          ${m(
      (b = this.value) == null ? void 0 : b.fieldType.supportsUploadTypes,
      () => {
        var E, J;
        return n`<umb-property-layout
                alias="allowedUploadTypes"
                .label=${this.localize.term(
          "fieldSettings_allowedFileUploadTypes"
        )}
              >
                <form-edit-allowed-file-upload-types
                  slot="editor"
                  .value=${this.value.allowedUploadTypes ?? []}
                  @change=${P(this, js, Qm)}
                ></form-edit-allowed-file-upload-types>
              </umb-property-layout>
              <umb-property-layout
                alias="allowMultipleFileUploads"
                .label=${this.localize.term(
          "fieldSettings_allowMultipleFileUploads"
        )}
              >
                <uui-toggle
                  slot="editor"
                  ?checked=${(E = this.value) == null ? void 0 : E.allowMultipleFileUploads}
                  label=${(J = this.value) != null && J.allowedUploadTypes ? "Multiple files" : "Single file only"}
                  @change=${P(this, Hs, Zm)}
                ></uui-toggle>
              </umb-property-layout>`;
      }
    )}
          ${m(
      (I = this.value) == null ? void 0 : I.fieldType.supportsPrevalues,
      () => {
        var E, J, _e, U;
        return n`<umb-property-layout
              alias="prevalues"
              .label=${this.localize.term("fieldSettings_prevalues")}
              .description=${this.localize.term(
          (((J = (E = this.data) == null ? void 0 : E.prevalueSources) == null ? void 0 : J.length) ?? 0) > 0 ? "fieldSettings_prevaluesProvideWithSources" : "fieldSettings_prevaluesProvide"
        )}
            >
              <div slot="editor">
                ${m(
          !this.value.prevalueSourceId || this.value.prevalueSourceId === Dt,
          () => n`<form-edit-prevalues
                      .value=${this.value.prevalues}
                      @change=${P(this, Ys, ep)}
                    ></form-edit-prevalues>`
        )}
                ${m(
          (((U = (_e = this.data) == null ? void 0 : _e.prevalueSources) == null ? void 0 : U.length) ?? 0) > 0,
          () => n`<div id="prevalueSource">
                    <label for="prevalueSource"
                      >${this.localize.term(
            "fieldSettings_prevaluesSource"
          )}</label
                    >
                    <uui-select
                      label="Prevalue Source"
                      name="prevalueSource"
                      @change=${P(this, Xs, op)}
                      .options=${P(this, Ks, tp).call(this)}
                    >
                    </uui-select>
                  </div>`
        )}
              </div>
            </umb-property-layout>`;
      }
    )}

          ${m(
      le(this, Q) && this.value && this._settingsConfigLoaded,
      () => n`
              <umb-property-dataset
                .value=${this._settingValues}
                @change=${P(this, Bs, Xm)}
              >
                ${this.value.fieldType.settings.map(
        (E) => n`
                    <umb-property
                      ?inert=${E.isReadOnly}
                      .label=${le(this, Q).getLocalizedSettingDetail(
          E,
          "Label",
          E.name
        )}
                      .description=${le(this, Q).getLocalizedSettingDetail(
          E,
          "Description",
          E.description
        )}
                      alias=${E.alias}
                      .config=${le(this, Q).getPropertyConfigForSetting(
          this._settingsConfig,
          E
        )}
                      .appearance=${le(this, Q).getPropertyAppearanceForSetting(
          E.view
        )}
                      property-editor-ui-alias=${E.view}
                    >
                    </umb-property>
                  `
      )}
              </umb-property-dataset>`
    )}

          ${m(
      (B = this.value) == null ? void 0 : B.fieldType.supportsMandatory,
      () => {
        var E, J, _e;
        return n`<umb-property-layout
              alias="mandatory"
              .label=${this.localize.term("fieldSettings_mandatory")}
            >
              <div slot="editor">
                <uui-toggle
                  ?checked=${(E = this.value) == null ? void 0 : E.mandatory}
                  .label=${(J = this.value) != null && J.mandatory ? "On" : "Off"}
                  @change=${P(this, Qs, ip)}
                ></uui-toggle>
                ${m(
          (_e = this.value) == null ? void 0 : _e.mandatory,
          () => {
            var U;
            return n`<uui-input
                    id="requiredErrorMessage"
                    name="requiredErrorMessage"
                    .value=${(U = this.value) == null ? void 0 : U.requiredErrorMessage}
                    @change=${P(this, Js, rp)}
                    label="Error message"
                    placeholder=${this.localize.term(
              "validation_mandatoryMessage"
            )}
                  ></uui-input>`;
          }
        )}
              </div>
            </umb-property-layout>`;
      }
    )}
          <!-- TODO => make this a component-->
          ${m(
      (Y = this.value) == null ? void 0 : Y.fieldType.supportsRegex,
      () => {
        var E, J, _e;
        return n`<umb-property-layout
              alias="regex"
              .label=${this.localize.term("formSettings_validation")}
            >
              <div slot="editor">
                <uui-select
                  name="regex"
                  @change=${P(this, Zs, ap)}
                  .options=${[
          {
            name: "",
            value: "",
            selected: !this.value.regex
          },
          ...((E = this.data) == null ? void 0 : E.validationPatterns.map((U) => ({
            name: U.labelKey.length > 0 ? this.localize.term(U.labelKey) : U.name,
            value: U.pattern,
            selected: U.pattern === this.value.regex
          }))) ?? [],
          {
            name: this.localize.term(
              "validation_enterCustomValidation"
            ),
            value: "custom",
            selected: P(this, en, sp).call(this)
          }
        ]}
                >
                </uui-select>

                ${m(
          this._showRegex,
          () => {
            var U;
            return n`
                    <textarea
                      placeholder=${this.localize.term(
              "fieldSettings_enterRegex"
            )}
                      ?disabled=${this._showRegexReadonly}
                      @change=${P(this, tn, np)}
                    >
${(U = this.value) == null ? void 0 : U.regex}</textarea
                    >
                  `;
          }
        )}
                ${m(
          (((_e = (J = this.value) == null ? void 0 : J.regex) == null ? void 0 : _e.length) ?? 0) > 0,
          () => {
            var U;
            return n`
                    <uui-input
                      id="invalidErrorMessage"
                      name="invalidErrorMessage"
                      .value=${(U = this.value) == null ? void 0 : U.invalidErrorMessage}
                      @change=${P(this, on, lp)}
                      label="Error message"
                      placeholder=${this.localize.term(
              "validation_mandatoryMessage"
            )}
                    ></uui-input>
                  `;
          }
        )}
              </div>
            </umb-property-layout>`;
      }
    )}

          <umb-property-layout
            alias="conditions"
            .label=${this.localize.term("formConditions_title")}
          >
            <div slot="editor">
              <uui-toggle
                ?checked=${this._conditionEnabled}
                .label=${this._conditionEnabled ? "On" : "Off"}
                @change=${P(this, rn, cp)}
              ></uui-toggle>
              ${m(
      this._conditionEnabled,
      () => {
        var E;
        return n`<form-edit-conditions
                    .value=${this.value.condition}
                    .fields=${((E = this.data) == null ? void 0 : E.fields) ?? []}
                    .appliedTo=${Xi.FIELD}
                    @change=${P(this, an, up)}
                  ></form-edit-conditions>`;
      }
    )}
            </div>
          </umb-property-layout>
        </uui-box>
      </div>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          @click=${P(this, sn, dp)}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
Q = /* @__PURE__ */ new WeakMap();
hr = /* @__PURE__ */ new WeakMap();
Is = /* @__PURE__ */ new WeakSet();
qm = async function() {
  var e;
  const t = { ...this.value.settings };
  (e = this.value) == null || e.fieldType.settings.forEach((o) => {
    t[o.alias] || (t[o.alias] = o.defaultValue);
  }), this._settingValues = await le(this, Q).getSettingValues(t);
};
Us = /* @__PURE__ */ new WeakSet();
Vm = function(t) {
  var i, a;
  const e = t.target.value.toString(), o = this.value.caption, r = this.value.alias;
  this._aliasLocked && Ru(o ?? "") === r && ((i = this.modalContext) == null || i.updateValue({ alias: Ru(e) })), (a = this.modalContext) == null || a.updateValue({ caption: e });
};
Ws = /* @__PURE__ */ new WeakSet();
Bm = function() {
  this._aliasLocked = !this._aliasLocked;
};
Ls = /* @__PURE__ */ new WeakSet();
jm = function(t) {
  var o;
  const e = t.target.value.toString();
  (o = this.modalContext) == null || o.updateValue({ alias: e });
};
zs = /* @__PURE__ */ new WeakSet();
Gm = function(t) {
  var o;
  const e = t.target.value.toString();
  (o = this.modalContext) == null || o.updateValue({ tooltip: e });
};
Ns = /* @__PURE__ */ new WeakSet();
Hm = async function() {
  (await this.getContext(D)).open(
    this,
    Bd
  ).onSubmit().then(async (o) => {
    var r, i;
    o.selectedValue && ((r = this.modalContext) == null || r.updateValue({ fieldType: o.selectedValue }), (i = le(this, Q)) == null || i.setProviderType(o.selectedValue), this.requestUpdate());
  }).catch(() => {
  });
};
qs = /* @__PURE__ */ new WeakSet();
Ym = function() {
  const t = [];
  if (!this.data)
    return t;
  for (let e = 0; e < this.data.pages.length; e++) {
    const o = this.data.pages[e];
    for (let r = 0; r < o.fieldSets.length; r++) {
      const i = o.fieldSets[r];
      for (let a = 0; a < i.containers.length; a++) {
        const s = i.containers[a], u = e + "_" + r + "_" + a;
        t.push({
          id: u,
          name: (o.caption || "Page " + (e + 1)) + " > " + (i.caption || "Group " + (r + 1)) + " > " + (s.caption || "Container " + (a + 1)),
          selected: this.value.containerIndexPath === u
        });
      }
    }
  }
  return t;
};
Vs = /* @__PURE__ */ new WeakSet();
Km = function(t) {
  var o;
  const e = t.target.value.toString();
  (o = this.modalContext) == null || o.updateValue({ containerIndexPath: e });
};
Bs = /* @__PURE__ */ new WeakSet();
Xm = async function(t) {
  const e = t.target.value;
  this._settingValues = e;
};
js = /* @__PURE__ */ new WeakSet();
Qm = function(t) {
  var o;
  const e = t.target.value;
  (o = this.modalContext) == null || o.updateValue({ allowedUploadTypes: e });
};
Gs = /* @__PURE__ */ new WeakSet();
Jm = function(t) {
  var o;
  const e = t.target.checked;
  (o = this.modalContext) == null || o.updateValue({ containsSensitiveData: e });
};
Hs = /* @__PURE__ */ new WeakSet();
Zm = function(t) {
  var o;
  const e = t.target.checked;
  (o = this.modalContext) == null || o.updateValue({ allowMultipleFileUploads: e });
};
Ys = /* @__PURE__ */ new WeakSet();
ep = function(t) {
  var o;
  const e = t.target.value;
  (o = this.modalContext) == null || o.updateValue({ prevalues: e });
};
Ks = /* @__PURE__ */ new WeakSet();
tp = function() {
  var e;
  const t = ((e = this.data) == null ? void 0 : e.prevalueSources.map((o) => ({
    name: o.name,
    value: o.id,
    selected: o.id === this.value.prevalueSourceId
  }))) ?? [];
  return t.unshift({
    name: "---" + this.localize.term("general_choose") + "---",
    value: Dt,
    selected: this.value.prevalueSourceId === Dt
  }), t;
};
Xs = /* @__PURE__ */ new WeakSet();
op = function(t) {
  var o;
  const e = t.target.value.toString();
  (o = this.modalContext) == null || o.updateValue({ prevalueSourceId: e });
};
Qs = /* @__PURE__ */ new WeakSet();
ip = function(t) {
  var o;
  const e = t.target.checked;
  (o = this.modalContext) == null || o.updateValue({ mandatory: e });
};
Js = /* @__PURE__ */ new WeakSet();
rp = function(t) {
  var o;
  const e = t.target.value.toString();
  (o = this.modalContext) == null || o.updateValue({ requiredErrorMessage: e });
};
Zs = /* @__PURE__ */ new WeakSet();
ap = function(t) {
  var o;
  const e = t.target.value.toString();
  (o = this.modalContext) == null || o.updateValue({ regex: e }), P(this, Ir, Sc).call(this);
};
en = /* @__PURE__ */ new WeakSet();
sp = function() {
  return this.value.regex ? P(this, Fa, wc).call(this, this.value.regex) === void 0 : !1;
};
tn = /* @__PURE__ */ new WeakSet();
np = function(t) {
  var r;
  const o = t.target.value.trim();
  (r = this.modalContext) == null || r.updateValue({ regex: o.length > 0 ? o : null });
};
Ir = /* @__PURE__ */ new WeakSet();
Sc = function() {
  var t, e;
  if ((e = (t = this.value) == null ? void 0 : t.regex) != null && e.length) {
    const o = P(this, Fa, wc).call(this, this.value.regex);
    this._showRegex = !0, this._showRegexReadonly = o !== void 0;
  } else
    this._showRegex = !1, this._showRegexReadonly = !1;
};
Fa = /* @__PURE__ */ new WeakSet();
wc = function(t) {
  var e;
  return (e = this.data) == null ? void 0 : e.validationPatterns.find((o) => o.pattern === t);
};
on = /* @__PURE__ */ new WeakSet();
lp = function(t) {
  var o;
  const e = t.target.value.toString();
  (o = this.modalContext) == null || o.updateValue({ invalidErrorMessage: e });
};
rn = /* @__PURE__ */ new WeakSet();
cp = function(t) {
  var o;
  this._conditionEnabled = t.target.checked;
  const e = structuredClone(this.value.condition);
  e.enabled = this._conditionEnabled, (o = this.modalContext) == null || o.updateValue({ condition: e });
};
an = /* @__PURE__ */ new WeakSet();
up = function(t) {
  var o;
  const e = t.target.value ?? void 0;
  (o = this.modalContext) == null || o.updateValue({ condition: e });
};
sn = /* @__PURE__ */ new WeakSet();
dp = async function() {
  var r;
  const t = await le(this, Q).getUpdatedSettingsForPersistence(
    this._settingValues,
    this.value.fieldType.settings
  ), e = {
    id: this.value.fieldType.id,
    requestBody: {
      caption: this.value.caption,
      alias: this.value.alias,
      settings: t,
      allowedUploadTypes: this.value.allowedUploadTypes
    }
  }, { error: o } = await lo(
    K.postFormFieldByIdValidateSettings(e)
  );
  if (o) {
    const i = le(this, Q).createValidationErrorNotification(
      "formEdit_saveFieldFailedTitle",
      o
    ), a = await this.getContext(Fo);
    a == null || a.peek("danger", i);
  } else
    (r = this.modalContext) == null || r.updateValue({ settings: t }), this._submitModal();
};
ue.styles = [
  C`
      #caption {
        display: flex;
        flex: 1 1 auto;
        margin-bottom: var(--uui-size-6);
      }

      uui-toggle {
        display: block;
      }

      textarea {
        box-sizing:border-box;
        width:100%;
      }

      textarea + *,
      uui-select + *,
      uui-toggle + * {
        display: block;
        margin-top: var(--uui-size-3);
      }

      #prevalueSource {
        margin-top: var(--uui-size-5);
      }

      [inert] {
        opacity: 0.5;
      }
    `
];
lt([
  w()
], ue.prototype, "_aliasLocked", 2);
lt([
  w()
], ue.prototype, "_settingValues", 2);
lt([
  w()
], ue.prototype, "_settingsConfig", 2);
lt([
  w()
], ue.prototype, "_settingsConfigLoaded", 2);
lt([
  w()
], ue.prototype, "_showRegex", 2);
lt([
  w()
], ue.prototype, "_showRegexReadonly", 2);
lt([
  w()
], ue.prototype, "_conditionEnabled", 2);
ue = lt([
  h(z_)
], ue);
const N_ = ue, q_ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsEditFieldModalElement() {
    return ue;
  },
  default: N_
}, Symbol.toStringTag, { value: "Module" }));
var V_ = Object.defineProperty, B_ = Object.getOwnPropertyDescriptor, Ea = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? B_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && V_(e, o, i), i;
}, j_ = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, ut = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, De = (t, e, o) => (j_(t, e, "access private method"), o), Yo, Ur, nn, mp, bc, pp, Fc, hp, Ec, fp, ln, yp, cn, gp;
const G_ = "form-edit-submit-message-modal";
let Mt = class extends ge {
  constructor() {
    super(...arguments), ut(this, Yo), ut(this, nn), ut(this, bc), ut(this, Fc), ut(this, Ec), ut(this, ln), ut(this, cn), this._richTextConfiguration = [], this._value = [];
  }
  connectedCallback() {
    var t;
    super.connectedCallback(), this._richTextConfiguration = (t = this.data) == null ? void 0 : t.richTextConfiguration, this._value = Object.keys(this.value ?? {}).map((e) => ({
      alias: e,
      value: this.value[e]
    })), De(this, nn, mp).call(this);
  }
  render() {
    return n`<umb-body-layout
      headline=${this.localize.term("formEdit_editSubmitMessage")}
    >
      <uui-box>
        <umb-property-dataset .value=${this._value} @change=${De(this, ln, yp)}>
          <umb-property
            .alias=${"messageOnSubmitIsHtml"}
            .label=${this.localize.term("formWorkflows_messageOnSubmitFormat")}
            .config=${[
      {
        alias: "showLabels",
        value: !0
      },
      {
        alias: "labelOn",
        value: this.localize.term(
          "formWorkflows_messageOnSubmitIsHtmlToggleTextOn"
        )
      },
      {
        alias: "labelOff",
        value: this.localize.term(
          "formWorkflows_messageOnSubmitIsHtmlToggleTextOff"
        )
      }
    ]}
            property-editor-ui-alias="Umb.PropertyEditorUi.Toggle"
          ></umb-property>
          <umb-property
            .alias=${"messageOnSubmit"}
            .label=${this.localize.term("formWorkflows_messageOnSubmit")}
            .description=${this.localize.term(
      "formWorkflows_messageOnSubmitDescription"
    )}
            .appearance=${{ labelOnTop: this._isHtml ?? !1 }}
            .config=${this._richTextConfiguration}
            .propertyEditorUiAlias=${`Umb.PropertyEditorUi.${this._isHtml ? "Tiptap" : "TextArea"}`}
          >
          </umb-property>
          <umb-property 
            .alias=${"goToPageOnSubmit"}
            .label=${this.localize.term("formWorkflows_goToPage")}
            .description=${this.localize.term(
      "formWorkflows_goToPageDescription"
    )}
            .config=${[{ alias: "validationLimit", value: { max: 1 } }]}
            property-editor-ui-alias="Umb.PropertyEditorUi.DocumentPicker"></umb-property>
          </umb-property>        
        </umb-property-dataset>
      </uui-box>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          @click=${De(this, cn, gp)}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
Yo = /* @__PURE__ */ new WeakSet();
Ur = function(t, e) {
  return t.find((o) => o.alias === e);
};
nn = /* @__PURE__ */ new WeakSet();
mp = function() {
  if (this._isHtml = this.value.messageOnSubmitIsHtml, this._isHtml) {
    const t = De(this, Yo, Ur).call(this, this._value, "messageOnSubmit");
    t.value = {
      markup: t == null ? void 0 : t.value
    };
  }
};
bc = /* @__PURE__ */ new WeakSet();
pp = function(t) {
  if (!t.trim().length)
    return "";
  let o = "<p>" + t.replace(/\r/g, "").split(/\n\n/).join("</p><p>") + "</p>";
  return o = o.replace(/\n/g, "<br/>"), { markup: o };
};
Fc = /* @__PURE__ */ new WeakSet();
hp = function(t) {
  return new DOMParser().parseFromString(t, "text/html").body.textContent || "";
};
Ec = /* @__PURE__ */ new WeakSet();
fp = function(t) {
  return this._isHtml ? typeof t == "string" ? De(this, bc, pp).call(this, t) : t : typeof t == "object" ? De(this, Fc, hp).call(this, t == null ? void 0 : t.markup) : t;
};
ln = /* @__PURE__ */ new WeakSet();
yp = function(t) {
  var r, i;
  let e = t.target.value;
  this._isHtml = !!((r = De(this, Yo, Ur).call(this, e, "messageOnSubmitIsHtml")) != null && r.value);
  const o = (i = De(this, Yo, Ur).call(this, e, "messageOnSubmit")) == null ? void 0 : i.value;
  e = qt(
    e,
    {
      alias: "messageOnSubmit",
      value: De(this, Ec, fp).call(this, o)
    },
    (a) => a.alias === "messageOnSubmit"
  ), this._value = e;
};
cn = /* @__PURE__ */ new WeakSet();
gp = function() {
  var e;
  const t = Object.fromEntries(
    this._value.map(({ alias: o, value: r }) => [o, r])
  );
  typeof t.messageOnSubmit == "object" && (t.messageOnSubmit = ((e = t.messageOnSubmit) == null ? void 0 : e.markup) ?? ""), this.updateValue(t), this._submitModal();
};
Ea([
  w()
], Mt.prototype, "_richTextConfiguration", 2);
Ea([
  w()
], Mt.prototype, "_value", 2);
Ea([
  w()
], Mt.prototype, "_isHtml", 2);
Mt = Ea([
  h(G_)
], Mt);
const H_ = Mt, Y_ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsEditSubmitMessageModalElement() {
    return Mt;
  },
  default: H_
}, Symbol.toStringTag, { value: "Module" }));
var K_ = Object.defineProperty, X_ = Object.getOwnPropertyDescriptor, _p = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? X_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && K_(e, o, i), i;
}, Tc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Gt = (t, e, o) => (Tc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), me = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Za = (t, e, o, r) => (Tc(t, e, "write to private field"), e.set(t, o), o), oe = (t, e, o) => (Tc(t, e, "access private method"), o), Ko, $c, Wr, Xo, un, vp, Ht, zo, fr, dn, mn, Sp, pn, wp, hn, bp, Ji, Ta;
const Q_ = "form-edit-allowed-file-upload-types";
let fn = class extends be {
  constructor() {
    super(), me(this, un), me(this, Ht), me(this, fr), me(this, mn), me(this, pn), me(this, hn), me(this, Ji), this._value = [], me(this, Ko, void 0), me(this, $c, [
      {
        name: "Allow all files",
        type: ""
      },
      {
        name: "PDF",
        type: "pdf"
      },
      {
        name: "DOCX",
        type: "docx"
      },
      {
        name: "XLSX",
        type: "xlsx"
      },
      {
        name: "TXT",
        type: "txt"
      },
      {
        name: "PNG",
        type: "png"
      },
      {
        name: "JPG",
        type: "jpg"
      },
      {
        name: "GIF",
        type: "gif"
      }
    ]), me(this, Wr, []), me(this, Xo, []), this.consumeContext(Fo, (t) => {
      Za(this, Ko, t);
    }), this.consumeContext(Qi, (t) => {
      t && this.observe(t.config, (e) => {
        e && (Za(this, Wr, e == null ? void 0 : e.disallowedFileUploadExtensions.split(",").filter(Boolean)), Za(this, Xo, e == null ? void 0 : e.allowedFileUploadExtensions.split(",").filter(Boolean)));
      });
    });
  }
  set value(t) {
    this._value = structuredClone(t), oe(this, un, vp).call(this);
  }
  get value() {
    return this._value;
  }
  render() {
    return n`<div>
      ${this.value.filter((t) => t.checked.length > 0).map(
      (t, e) => n`<div>
              <uui-toggle
                ?checked=${t.checked === "true"}
                ?disabled=${e > 0 && oe(this, fr, dn).call(this)}
                .label=${t.name}
                @change=${() => oe(this, mn, Sp).call(this, e)}
              ></uui-toggle>
            </div>`
    )}
      <div>
        <b
          >${this.localize.term(
      "formFileUpload_userDefinedAllowedFileTypes"
    )}</b
        >
      </div>
      <div>
        ${this.value.filter((t) => t.checked.length === 0).map(
      (t, e) => n`<div>
                <span>${t.name}</span>
                <uui-button
                  label=${this.localize.term("general_delete")}
                  look="secondary"
                  color="default"
                  @click=${() => oe(this, hn, bp).call(this, e)}
                >
                  <uui-icon name="icon-delete"></uui-icon>
                </uui-button>
              </div>`
    )}
      </div>
      <form>
        <uui-input
          id="addNew"
          type="text"
          size="30"
          .placeholder=${this.localize.term("formFileUpload_addAllowedFileType")}
        ></uui-input>
        <uui-button
          label=${this.localize.term("general_add")}
          look="secondary"
          color="default"
          .disabled="${oe(this, fr, dn).call(this)}"
          @click=${() => oe(this, pn, wp).call(this)}
        >
          <uui-icon name="icon-add"></uui-icon>
        </uui-button>
      </form>
    </div>`;
  }
};
Ko = /* @__PURE__ */ new WeakMap();
$c = /* @__PURE__ */ new WeakMap();
Wr = /* @__PURE__ */ new WeakMap();
Xo = /* @__PURE__ */ new WeakMap();
un = /* @__PURE__ */ new WeakSet();
vp = function() {
  Gt(this, $c).filter((e) => oe(this, Ht, zo).call(this, e.type)).forEach((e) => {
    this._value.find((o) => o.type === e.type) || this._value.push({
      name: e.name,
      type: e.type,
      checked: "false"
    });
  }), this._value.filter((e) => oe(this, Ht, zo).call(this, e.type)).length > 0 && (this._value = this._value.filter((e) => oe(this, Ht, zo).call(this, e.type)));
};
Ht = /* @__PURE__ */ new WeakSet();
zo = function(t) {
  return t === "" ? !0 : !(Gt(this, Wr).includes(t) || Gt(this, Xo).length > 0 && Gt(this, Xo).includes(t) === !1);
};
fr = /* @__PURE__ */ new WeakSet();
dn = function() {
  return this.value[0].checked === "true";
};
mn = /* @__PURE__ */ new WeakSet();
Sp = function(t) {
  const e = this._value[t].checked;
  this._value[t].checked = e === "true" ? "false" : "true", oe(this, Ji, Ta).call(this);
};
pn = /* @__PURE__ */ new WeakSet();
wp = function() {
  var r, i, a;
  const t = (r = this.shadowRoot) == null ? void 0 : r.getElementById(
    "addNew"
  ), e = t.value.toString().replace(/[^a-zA-Z0-9]/g, "").toLowerCase();
  if (t.value = "", e.length === 0)
    return;
  if (!oe(this, Ht, zo).call(this, e)) {
    (i = Gt(this, Ko)) == null || i.peek("danger", {
      data: {
        headline: this.localize.term("formFileUpload_disallowedFileExtensionErrorTitle"),
        message: this.localize.term("formFileUpload_disallowedFileExtensionErrorMessage")
      }
    });
    return;
  }
  if (this._value.findIndex(function(s) {
    return s.type.toLowerCase() === e;
  }) >= 0) {
    (a = Gt(this, Ko)) == null || a.peek("danger", {
      data: {
        headline: this.localize.term("formFileUpload_duplicateFileTypeErrorTitle"),
        message: this.localize.term("formFileUpload_duplicateFileTypeErrorMessage")
      }
    });
    return;
  }
  this._value.push({
    type: e,
    name: e.toUpperCase(),
    checked: ""
  }), oe(this, Ji, Ta).call(this);
};
hn = /* @__PURE__ */ new WeakSet();
bp = function(t) {
  const e = this.value.filter(
    (o) => o.checked.length > 0
  ).length;
  this._value.splice(t + e, 1), oe(this, Ji, Ta).call(this);
};
Ji = /* @__PURE__ */ new WeakSet();
Ta = function() {
  this.requestUpdate(), this.dispatchEvent(
    new CustomEvent("change", { composed: !0, bubbles: !0 })
  );
};
_p([
  p()
], fn.prototype, "value", 1);
fn = _p([
  h(Q_)
], fn);
var J_ = Object.defineProperty, Z_ = Object.getOwnPropertyDescriptor, Cc = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? Z_(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && J_(e, o, i), i;
}, Oc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Lr = (t, e, o) => (Oc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), ee = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Fp = (t, e, o, r) => (Oc(t, e, "write to private field"), e.set(t, o), o), x = (t, e, o) => (Oc(t, e, "access private method"), o), yt, Pc, yn, Ep, No, zr, Zi, $a, er, Ca, Oa, kc, Pa, Dc, gn, Tp, _n, $p, vn, Cp, Sn, Op, Qo, ka;
const ev = "form-edit-prevalues";
let Jo = class extends be {
  constructor() {
    super(...arguments), ee(this, yn), ee(this, No), ee(this, Zi), ee(this, er), ee(this, Oa), ee(this, Pa), ee(this, gn), ee(this, _n), ee(this, vn), ee(this, Sn), ee(this, Qo), ee(this, yt, []), ee(this, Pc, new ji(this, {
      ...new Yi(
        "Forms.SorterIdentifier.Prevalues",
        "tr.prevalue-row",
        "tbody.prevalues-container"
      ).config,
      onChange: ({ model: t }) => {
        Fp(this, yt, t);
      },
      onEnd: () => {
        const t = structuredClone(this._value);
        t.sort(
          (e, o) => Lr(this, yt).indexOf(e.value) - Lr(this, yt).indexOf(o.value)
        ), this._value = t, x(this, Qo, ka).call(this);
      }
    })), this._value = [], this._editIndex = -1;
  }
  set value(t) {
    this._value = structuredClone(t), x(this, yn, Ep).call(this);
  }
  get value() {
    return this._value;
  }
  render() {
    return n` <table>
      <thead>
        <tr>
          <th></th>
          <th>Value</th>
          <th>Caption</th>
          <th></th>
        </tr>
      </thead>
      <tbody class="prevalues-container">
        ${this.value.map(
      (t, e) => n`<tr class="prevalue-row" sort-unique=${t.value}>
              <td><uui-icon name="icon-navigation"></uui-icon></td>
              <td>${t.value}</td>
              <td>${t.caption}</td>
              <td>
                <uui-action-bar>
                  <uui-button
                    label="edit"
                    look="secondary"
                    color="default"
                    @click=${() => x(this, gn, Tp).call(this, e)}
                  >
                    <uui-icon name="edit"></uui-icon>
                  </uui-button>
                  <uui-button
                    label=${this.localize.term("general_delete")}
                    look="secondary"
                    color="default"
                    @click=${() => x(this, Sn, Op).call(this, e)}
                  >
                    <uui-icon name="delete"></uui-icon>
                  </uui-button>
                </uui-action-bar>
              </td>
            </tr>`
    )}

        <tr>
          <td></td>
          <td>
            <uui-input
              id="value"
              name="value"
              type="text"
              size="30"
              maxlength="255"
              .placeholder=${this.localize.term("formPrevalues_newValue")}
            ></uui-input>
          </td>
          <td>
            <uui-input
              id="caption"
              name="caption"
              type="text"
              size="30"
              maxlength="255"
              .placeholder=${this.localize.term("formPrevalues_newCaption")}
            ></uui-input>
          </td>
          <td>
            <uui-action-bar>
              <uui-button
                label="add"
                look="secondary"
                color="default"
                @click=${x(this, _n, $p)}
              >
                <uui-icon
                  .name=${x(this, No, zr).call(this) ? "icon-save" : "add"}
                ></uui-icon>
              </uui-button>
              ${m(
      x(this, No, zr).call(this),
      () => n`<uui-button
                  label="add"
                  look="secondary"
                  color="default"
                  @click=${x(this, vn, Cp)}
                  ><uui-icon name="wrong"></uui-icon
                ></uui-button>`
    )}
            </uui-action-bar>
          </td>
        </tr>
      </tbody>
    </table>`;
  }
};
yt = /* @__PURE__ */ new WeakMap();
Pc = /* @__PURE__ */ new WeakMap();
yn = /* @__PURE__ */ new WeakSet();
Ep = function() {
  Fp(this, yt, this._value.map((t) => t.value)), Lr(this, Pc).setModel(Lr(this, yt));
};
No = /* @__PURE__ */ new WeakSet();
zr = function() {
  return this._editIndex > -1;
};
Zi = /* @__PURE__ */ new WeakSet();
$a = function() {
  return x(this, Oa, kc).call(this, "value");
};
er = /* @__PURE__ */ new WeakSet();
Ca = function() {
  return x(this, Oa, kc).call(this, "caption");
};
Oa = /* @__PURE__ */ new WeakSet();
kc = function(t) {
  var e;
  return (e = this.shadowRoot) == null ? void 0 : e.getElementById(t);
};
Pa = /* @__PURE__ */ new WeakSet();
Dc = function() {
  x(this, Zi, $a).call(this).value = "", x(this, er, Ca).call(this).value = "", this._editIndex = -1;
};
gn = /* @__PURE__ */ new WeakSet();
Tp = function(t) {
  this._editIndex = t;
  const e = this._value[this._editIndex];
  x(this, Zi, $a).call(this).value = e.value, x(this, er, Ca).call(this).value = e.caption || "";
};
_n = /* @__PURE__ */ new WeakSet();
$p = function() {
  const t = x(this, Zi, $a).call(this).value, e = x(this, er, Ca).call(this).value;
  x(this, No, zr).call(this) ? (this._value[this._editIndex].value = t, this._value[this._editIndex].caption = e) : this._value.push({
    value: t,
    caption: e
  }), x(this, Pa, Dc).call(this), x(this, Qo, ka).call(this);
};
vn = /* @__PURE__ */ new WeakSet();
Cp = function() {
  x(this, Pa, Dc).call(this);
};
Sn = /* @__PURE__ */ new WeakSet();
Op = async function(t) {
  await Yl(this, {
    headline: this.localize.term("formPrevalues_deletePrevalueHeadline"),
    content: this.localize.term("formPrevalues_deletePrevalueMessage"),
    confirmLabel: this.localize.term("general_yes"),
    color: "danger"
  }), this._value.splice(t, 1), x(this, Qo, ka).call(this);
};
Qo = /* @__PURE__ */ new WeakSet();
ka = function() {
  this.requestUpdate(), this.dispatchEvent(
    new CustomEvent("change", { composed: !0, bubbles: !0 })
  );
};
Jo.styles = [
  C`
      table {
        width: 100%;
      }

      th:last-child {
        width: 120px;
      }

      /* match padding to uui-input */
      td:not(:has(uui-input, uui-button)) {
        padding: var(--uui-size-1, 3px) var(--uui-size-space-3, 9px);
      }

      th {
        text-align: left;
      }

      .prevalue-row {
        cursor: move;
      }
    `
];
Cc([
  p({ type: Array })
], Jo.prototype, "value", 1);
Cc([
  w()
], Jo.prototype, "_editIndex", 2);
Jo = Cc([
  h(ev)
], Jo);
const Da = new M("UmbCollectionContext");
var tv = Object.defineProperty, ov = Object.getOwnPropertyDescriptor, xc = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? ov(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && tv(e, o, i), i;
}, Mc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, yr = (t, e, o) => (Mc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), lr = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, wn = (t, e, o, r) => (Mc(t, e, "write to private field"), e.set(t, o), o), Gu = (t, e, o) => (Mc(t, e, "access private method"), o), mo, qo, bn, Pp, Fn, kp;
const iv = "form-export-entries-modal";
let po = class extends ge {
  constructor() {
    super(...arguments), lr(this, bn), lr(this, Fn), this._exportTypes = [], lr(this, mo, void 0), lr(this, qo, !1);
  }
  async connectedCallback() {
    var t;
    super.connectedCallback(), this._exportTypes = await is.getExportTypes({
      formId: (t = this.data) == null ? void 0 : t.unique
    });
  }
  render() {
    return n`<umb-body-layout
      .headline=${this.localize.term("formEntries_export")}
    >
      <uui-box>
        <umb-property-layout
          orientation="vertical"
          .label=${this.localize.term("formEntries_chooseExportFormat")}
        >
          <div slot="editor">
            <uui-ref-list>
              ${this._exportTypes.map(
      (t) => n`<umb-ref-item
                    selectable
                    id=${t.alias}
                    .name=${this.localize.term(
        `formProviderExportTypes_${t.alias}`,
        t.isOsx
      )}
                    .detail=${this.localize.term(
        `formProviderExportTypes_${t.alias}Description`,
        t.isOsx
      )}
                    @selected=${Gu(this, bn, Pp)}
                  >
                    ${m(
        t.icon,
        () => n`<uui-icon name=${t.icon}></uui-icon>`
      )}
                  </umb-ref-item>`
    )}
            </uui-ref-list>
          </div>
        </umb-property-layout>
      </uui-box>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          look="primary"
          color="positive"
          label=${this.localize.term("actions_export")}
          @click=${Gu(this, Fn, kp)}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
mo = /* @__PURE__ */ new WeakMap();
qo = /* @__PURE__ */ new WeakMap();
bn = /* @__PURE__ */ new WeakSet();
Pp = function(t) {
  var o, r;
  const e = t.target;
  e.id !== yr(this, mo) && ((o = this._refItems) == null || o.forEach((i) => {
    i.id !== e.id && (i.selected = !1);
  }), wn(this, mo, (r = this._exportTypes.find(
    (i) => i.alias === e.id
  )) == null ? void 0 : r.id));
};
Fn = /* @__PURE__ */ new WeakSet();
kp = async function() {
  var s, u;
  if (yr(this, qo) || !yr(this, mo))
    return;
  wn(this, qo, !0);
  const t = await this.getContext(
    Da
  ), e = {
    ...await Kl(t.filter),
    exportType: yr(this, mo),
    formId: (s = this.data) == null ? void 0 : s.unique
  }, { formId: o, fileName: r } = await is.postExport({
    formId: (u = this.data) == null ? void 0 : u.unique,
    requestBody: e
  }), i = await is.getExport({ fileName: r, formId: o });
  Kf(i, r, "text/xml"), wn(this, qo, !1), (await this.getContext(Fo)).peek("positive", {
    data: { message: "Export complete." }
  }), this._submitModal();
};
xc([
  w()
], po.prototype, "_exportTypes", 2);
xc([
  wd("umb-ref-item")
], po.prototype, "_refItems", 2);
po = xc([
  h(iv)
], po);
const rv = po, av = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsExportEntriesModalElement() {
    return po;
  },
  default: rv
}, Symbol.toStringTag, { value: "Module" })), sv = new M(
  "UmbWorkspaceContext",
  void 0,
  (t) => {
    var e;
    return ((e = t.getEntityType) == null ? void 0 : e.call(t)) === Ut;
  }
), nv = "Forms.Repository.FieldType.Collection", lv = {
  type: "repository",
  alias: nv,
  name: "Field Type Collection Repository",
  api: () => Promise.resolve().then(() => Wg)
}, cv = [lv];
class uv extends jf {
  async execute() {
    const e = await this.getContext(sv), o = await this.getContext(D), r = new Or(this), { data: i } = await r.requestCollection(), a = (i == null ? void 0 : i.items) || [];
    await e.getWizardScaffold(), o.open(this, kg, {
      data: {
        fieldTypes: a
      },
      value: {
        wizard: await e.getWizardScaffold()
      }
    }).onSubmit().catch(() => {
    });
  }
}
const Zo = "Forms.Workspace.DataSource", dv = {
  type: "workspace",
  kind: "routable",
  alias: Zo,
  name: "Data Source Workspace",
  api: () => import("./datasource-workspace.context.js"),
  meta: {
    entityType: Ut
  }
}, mv = [
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.DataSource.Design",
    name: "Form Workspace Design View",
    element: () => import("./workspace-view-datasource-design.element.js"),
    weight: 90,
    meta: {
      label: "Design",
      pathname: "design",
      icon: "document"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Zo
      },
      {
        alias: ce,
        match: (t) => t.userSecurity.manageDataSources
      }
    ]
  },
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.DataSource.Info",
    name: "Form Workspace Info View",
    element: () => import("./workspace-view-datasource-info.element.js"),
    weight: 90,
    meta: {
      label: "Info",
      pathname: "info",
      icon: "info"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Zo
      },
      {
        alias: ce,
        match: (t) => t.userSecurity.manageDataSources
      }
    ]
  }
], pv = [
  {
    type: "workspaceAction",
    kind: "default",
    alias: "Forms.WorkspaceAction.DataSource.Save",
    name: "Save Data Source Workspace Action",
    api: Bo,
    meta: {
      label: "Save",
      look: "primary",
      color: "positive"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Zo
      }
    ]
  },
  {
    type: "workspaceAction",
    kind: "default",
    alias: "Forms.WorkspaceAction.DataSource.CreateForm",
    name: "Create Form From Data Source Workspace Action",
    api: uv,
    meta: {
      label: "Create Form"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Zo
      },
      {
        alias: Ad
      }
    ]
  }
], hv = [dv, ...mv, ...pv], fv = [
  ...Wy,
  ...ig,
  ...mg,
  ...Iy,
  ...hv
], yv = "Forms.Repository.DataSourceType.Collection", gv = {
  type: "repository",
  alias: yv,
  name: "Data Source Type Collection Repository",
  api: () => import("./datasource-type-collection.repository.js")
}, _v = [gv];
var Ci;
class vv {
  constructor(e) {
    f(this, Ci, void 0);
    _(this, Ci, e);
  }
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, Ci),
      Sy.getDataSourceTypeById({
        id: e
      })
    );
    return r || !o ? { error: r } : { data: o };
  }
  createScaffold(e) {
    throw new Error("Method not implemented.");
  }
  create(e, o) {
    throw new Error("Method not implemented.");
  }
  update(e) {
    throw new Error("Method not implemented.");
  }
  delete(e) {
    throw new Error("Method not implemented.");
  }
}
Ci = new WeakMap();
class Sv extends we {
  constructor(e) {
    super(
      e,
      Dp.toString()
    );
  }
}
const Dp = new M("FormsDataSourceTypeDetailStore");
class wv extends Se {
  constructor(e) {
    super(
      e,
      vv,
      Dp
    );
  }
}
const bv = "Forms.Repository.DataSourceType.Detail", Fv = "Forms.Store.DataSourceType.Detail", Ev = {
  type: "repository",
  alias: bv,
  name: "Data Source Type Detail Repository",
  api: wv
}, Tv = {
  type: "store",
  alias: Fv,
  name: "Field Data Source Type Detail Store",
  api: Sv
}, $v = [Ev, Tv], Cv = [
  ..._v,
  ...$v
];
var Oi;
class Ov {
  constructor(e) {
    f(this, Oi, void 0);
    _(this, Oi, e);
  }
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, Oi),
      ya.getFieldTypeById({
        id: e
      })
    );
    return r || !o ? { error: r } : { data: o };
  }
  createScaffold(e) {
    throw new Error("Method not implemented.");
  }
  create(e, o) {
    throw new Error("Method not implemented.");
  }
  update(e) {
    throw new Error("Method not implemented.");
  }
  delete(e) {
    throw new Error("Method not implemented.");
  }
}
Oi = new WeakMap();
class Pv extends we {
  constructor(e) {
    super(
      e,
      xp.toString()
    );
  }
}
const xp = new M("FormsFieldTypeDetailStore");
class kv extends Se {
  constructor(e) {
    super(
      e,
      Ov,
      xp
    );
  }
  async requestValidationPatterns() {
    const { data: e, error: o } = await y(this._host, ya.getFieldTypeValidationPattern());
    return o || !e ? { error: o } : { data: e };
  }
}
const Dv = "Forms.Repository.FieldType.Detail", xv = "Forms.Store.FieldType.Detail", Mv = {
  type: "repository",
  alias: Dv,
  name: "Field Type Detail Repository",
  api: kv
}, Av = {
  type: "store",
  alias: xv,
  name: "Field Type Detail Store",
  api: Pv
}, Rv = [Mv, Av], Iv = [
  ...cv,
  ...Rv
], Uv = "Forms.Repository.PrevalueSourceType.Collection", Wv = {
  type: "repository",
  alias: Uv,
  name: "Prevalue Source Type Collection Repository",
  api: () => import("./prevaluesource-type-collection.repository.js")
}, Lv = [Wv];
var Pi;
class zv {
  constructor(e) {
    f(this, Pi, void 0);
    _(this, Pi, e);
  }
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, Pi),
      Fy.getPrevalueSourceTypeById(
        { id: e }
      )
    );
    return r || !o ? { error: r } : { data: o };
  }
  createScaffold(e) {
    throw new Error("Method not implemented.");
  }
  create(e, o) {
    throw new Error("Method not implemented.");
  }
  update(e) {
    throw new Error("Method not implemented.");
  }
  delete(e) {
    throw new Error("Method not implemented.");
  }
}
Pi = new WeakMap();
class Nv extends we {
  constructor(e) {
    super(
      e,
      Mp.toString()
    );
  }
}
const Mp = new M("FormsPrevalueSourceTypeDetailStore");
class qv extends Se {
  constructor(e) {
    super(
      e,
      zv,
      Mp
    );
  }
}
const Vv = "Forms.Repository.PrevalueSourceType.Detail", Bv = "Forms.Store.PrevalueSourceType.Detail", jv = {
  type: "repository",
  alias: Vv,
  name: "Prevalue Source Type Detail Repository",
  api: qv
}, Gv = {
  type: "store",
  alias: Bv,
  name: "Field Prevalue Source Type Detail Store",
  api: Nv
}, Hv = [jv, Gv], Yv = [
  ...Lv,
  ...Hv
];
class Kv {
  async getSettingValueForEditor(e, o, r) {
    return Promise.resolve(r);
  }
  async getSettingValueForPersistence(e, o) {
    var r;
    if (Array.isArray(o.value)) {
      const i = o.value;
      return Promise.resolve((i == null ? void 0 : i.length) > 0 ? i[0] : "");
    } else
      return Promise.resolve(((r = o.value) == null ? void 0 : r.toString()) || "");
  }
  async getSettingPropertyConfig(e, o, r) {
    const i = [];
    return i.push({
      alias: "multiple",
      value: !1
    }), i.push({
      alias: "items",
      value: e.prevalues
    }), Promise.resolve(i);
  }
  destroy() {
  }
}
class Xv {
  async getSettingValueForEditor(e, o, r) {
    return Promise.resolve(r);
  }
  async getSettingValueForPersistence(e, o) {
    var r;
    return Promise.resolve(((r = o.value) == null ? void 0 : r.toString()) || "");
  }
  async getSettingPropertyConfig(e, o, r) {
    const i = [];
    return e.prevalues.length >= 1 && (i.push({
      alias: "min",
      value: e.prevalues[0]
    }), e.prevalues.length >= 2 && (i.push({
      alias: "max",
      value: e.prevalues[1]
    }), e.prevalues.length >= 3 && i.push({
      alias: "step",
      value: e.prevalues[2]
    }))), Promise.resolve(i);
  }
  destroy() {
  }
}
class Qv {
  async getSettingValueForEditor(e, o, r) {
    if (r) {
      const { data: i } = await lo(
        by.getMediaByPath({ path: encodeURIComponent(r) })
      );
      if (i)
        return Promise.resolve(i.id);
    }
    return Promise.resolve("");
  }
  async getSettingValueForPersistence(e, o) {
    var i, a;
    const r = o.value;
    if (r) {
      const { data: s } = await lo(
        Zf.getMediaUrls({ id: [r] })
      ), u = (a = (i = s == null ? void 0 : s[0]) == null ? void 0 : i.urlInfos) == null ? void 0 : a[0].url;
      if (!u)
        return Promise.resolve("");
      const S = new URL(u).pathname;
      return Promise.resolve(S);
    }
    return Promise.resolve("");
  }
  async getSettingPropertyConfig(e, o, r) {
    const i = [];
    return i.push({
      alias: "validationLimit",
      value: {
        min: 1,
        max: 1
      }
    }), Promise.resolve(i);
  }
  destroy() {
  }
}
class Jv {
  async getSettingValueForEditor(e, o, r) {
    if (!isNaN(parseFloat(r))) {
      const i = Math.trunc(parseFloat(r) * 10);
      return Promise.resolve({ from: i, to: i });
    }
    return Promise.resolve(void 0);
  }
  async getSettingValueForPersistence(e, o) {
    const r = ((o.value ? parseInt(o.value.from) : 5) / 10).toFixed(1);
    return Promise.resolve(r);
  }
  async getSettingPropertyConfig(e, o, r) {
    var s, u;
    const i = [];
    i.push({
      alias: "enableRange",
      value: !1
    });
    let a = ((u = (s = r.find((S) => S.alias === o)) == null ? void 0 : s.value) == null ? void 0 : u.toString()) || "";
    return isNaN(parseFloat(a)) && (a = ""), e.prevalues.length >= 1 && (i.push({
      alias: "minVal",
      value: parseFloat(e.prevalues[0]) * 10
    }), e.prevalues.length >= 2 && (i.push({
      alias: "maxVal",
      value: parseFloat(e.prevalues[1]) * 10
    }), e.prevalues.length >= 3 && (i.push({
      alias: "step",
      value: parseFloat(e.prevalues[2]) * 10
    }), e.prevalues.length >= 3 && a.length === 0 ? i.push({
      alias: "initVal1",
      value: parseFloat(e.prevalues[3]) * 10
    }) : i.push({
      alias: "initVal1",
      value: parseFloat(a)
    })))), Promise.resolve(i);
  }
  destroy() {
  }
}
class Hu {
  async getSettingValueForEditor(e, o, r) {
    const i = { markup: r };
    return Promise.resolve(i);
  }
  async getSettingValueForPersistence(e, o) {
    const r = o.value ? o.value.markup : "";
    return Promise.resolve(r);
  }
  async getSettingPropertyConfig(e, o, r) {
    const i = [], { data: a } = await lo(
      ya.getFieldTypeRichtextDatatype()
    );
    return i.push({
      alias: "maxImageSize",
      value: a.configurationData.maxImageSize
    }), i.push({
      alias: "toolbar",
      value: a.configurationData.toolbar
    }), a.configurationData.extensions && i.push({
      alias: "extensions",
      value: a.configurationData.extensions
    }), a.configurationData.mode && (i.push({
      alias: "mode",
      value: a.configurationData.mode
    }), a.configurationData.mode && i.push({
      alias: "stylesheets",
      value: a.configurationData.stylesheets
    })), i;
  }
  destroy() {
  }
}
class Zv {
  async getSettingValueForEditor(e, o, r) {
    let i = !1;
    return r ? i = r.toLowerCase() === "true" : i = e.prevalues.length >= 1 && e.prevalues[0].toLowerCase() === "true", Promise.resolve(i);
  }
  async getSettingValueForPersistence(e, o) {
    const r = o.value ? "True" : "False";
    return Promise.resolve(r);
  }
  async getSettingPropertyConfig(e, o, r) {
    const i = [];
    return Promise.resolve(i);
  }
  destroy() {
  }
}
class eS {
  async getSettingValueForEditor(e, o, r) {
    return Promise.resolve(r);
  }
  async getSettingValueForPersistence(e, o) {
    const r = o.value ? o.value.temporaryFileId : "";
    return Promise.resolve(r);
  }
  async getSettingPropertyConfig(e, o, r) {
    const i = [];
    return i.push({
      alias: "multiple",
      value: !1
    }), i.push({
      alias: "fileExtensions",
      value: ["txt"]
    }), Promise.resolve(i);
  }
  destroy() {
  }
}
class tS {
  async getSettingValueForEditor(e, o, r) {
    return r.length === 0 ? Promise.resolve({}) : Promise.resolve(JSON.parse(r));
  }
  async getSettingValueForPersistence(e, o) {
    return Promise.resolve(JSON.stringify(o.value));
  }
  async getSettingPropertyConfig(e, o, r) {
    return Promise.resolve([]);
  }
  destroy() {
  }
}
class oS {
  async getSettingValueForEditor(e, o, r) {
    let i = [];
    return r ? i = r.split(",") : i = [], Promise.resolve(i);
  }
  async getSettingValueForPersistence(e, o) {
    const r = o.value.join(",");
    return Promise.resolve(r);
  }
  async getSettingPropertyConfig(e, o, r) {
    const i = [];
    return Promise.resolve(i);
  }
  destroy() {
  }
}
class iS {
  async getSettingValueForEditor(e, o, r) {
    return Promise.resolve(r);
  }
  async getSettingValueForPersistence(e, o) {
    var r;
    return Promise.resolve(((r = o.value) == null ? void 0 : r.toString()) || "");
  }
  async getSettingPropertyConfig(e, o, r) {
    const i = [];
    return i.push({
      alias: "settingProvidingDocTypeAlias",
      value: e.prevalues.length > 0 ? e.prevalues[0] : ""
    }), Promise.resolve(i);
  }
  destroy() {
  }
}
const rS = [
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.Dropdown",
    name: "Dropdown Setting Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.Dropdown",
    api: Kv
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.Integer",
    name: "Number Setting Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.Integer",
    api: Xv
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.MediaEntityPicker",
    name: "Media Entity Picker Setting Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.MediaEntityPicker",
    api: Qv
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.Slider",
    name: "Number Slider Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.Slider",
    api: Jv
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.RichText.TinyMCE",
    name: "Rich Text Setting Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.TinyMCE",
    api: Hu
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.RichText.Tiptap",
    name: "Rich Text Setting Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.Tiptap",
    api: Hu
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.Toggle",
    name: "Number Toggle Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle",
    api: Zv
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.Upload",
    name: "Upload Setting Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.UploadField",
    api: eS
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.SourcePicker",
    name: "Source Picker Setting Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.ContentPicker.Source",
    api: tS
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.MultipleTextString",
    name: "Multiple Text String Setting Value Converter",
    propertyEditorUiAlias: "Umb.PropertyEditorUi.MultipleTextString",
    api: oS
  },
  {
    type: "formsSettingValueConverter",
    alias: "Forms.SettingValueConverter.DocumentTypeFieldPicker",
    name: "Document Type Field Picker Setting Value Converter",
    propertyEditorUiAlias: "Forms.PropertyEditorUi.DocumentTypeFieldPicker",
    api: iS
  }
], aS = "Forms.Repository.WorkflowType.Collection", sS = {
  type: "repository",
  alias: aS,
  name: "Workflow Type Collection Repository",
  api: () => Promise.resolve().then(() => Yg)
}, nS = [sS];
var ki;
class lS {
  constructor(e) {
    f(this, ki, void 0);
    _(this, ki, e);
  }
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, ki),
      Md.getWorkflowTypeById({
        id: e
      })
    );
    return r || !o ? { error: r } : { data: o };
  }
  createScaffold(e) {
    throw new Error("Method not implemented.");
  }
  create(e, o) {
    throw new Error("Method not implemented.");
  }
  update(e) {
    throw new Error("Method not implemented.");
  }
  delete(e) {
    throw new Error("Method not implemented.");
  }
}
ki = new WeakMap();
class cS extends we {
  constructor(e) {
    super(
      e,
      Ap.toString()
    );
  }
}
const Ap = new M("FormsWorkflowTypeDetailStore");
class uS extends Se {
  constructor(e) {
    super(
      e,
      lS,
      Ap
    );
  }
}
const dS = "Forms.Repository.WorkflowType.Detail", mS = "Forms.Store.WorkflowType.Detail", pS = {
  type: "repository",
  alias: dS,
  name: "Workflow Type Detail Repository",
  api: uS
}, hS = {
  type: "store",
  alias: mS,
  name: "Field Workflow Type Detail Store",
  api: cS
}, fS = [pS, hS], yS = [
  ...nS,
  ...fS
], gS = [
  ...Cv,
  ...Iv,
  ...Yv,
  ...rS,
  ...yS
], _S = "email-template", vS = "email-template-root", SS = "email-template-folder", Rp = "Forms.Repository.EmailTemplate.Tree", wS = "Forms.Store.EmailTemplate.Tree", bS = "Forms.Tree.EmailTemplate", FS = {
  type: "repository",
  alias: Rp,
  name: "Email Template Tree Repository",
  api: () => import("./email-template-tree.repository.js")
}, ES = {
  type: "treeStore",
  alias: wS,
  name: "Email Template Tree Store",
  api: () => import("./email-template-tree.store.js")
}, TS = {
  type: "tree",
  kind: "default",
  alias: bS,
  name: "Email Template Tree",
  meta: {
    repositoryAlias: Rp
  }
}, $S = {
  type: "treeItem",
  kind: "default",
  alias: "Forms.TreeItem.EmailTemplate",
  name: "Email Template Tree Item",
  forEntityTypes: [
    vS,
    _S,
    SS
  ]
}, CS = [
  FS,
  ES,
  TS,
  $S
], OS = [
  ...CS
], En = "Forms.Condition.FormSettings";
class PS extends $d {
  constructor(e, o) {
    super(e, o), this.consumeContext(Ae, (r) => {
      const i = r.getData();
      this.permitted = i ? this.config.match(i) : !1;
    });
  }
}
const kS = [
  {
    type: "condition",
    name: "Form Settings Condition",
    alias: En,
    api: PS
  }
], DS = [...kS];
class xS extends we {
  constructor(e) {
    super(
      e,
      Ip.toString()
    );
  }
}
const Ip = new M("FormDetailStore");
var Be;
class MS {
  constructor(e) {
    f(this, Be, void 0);
    _(this, Be, e);
  }
  /**
   * Creates a new Form scaffold
   * @param {(string | null)} parentUnique
   * @return { FormDetailModel }
   * @memberof FormsFormDetailServerDataSource
   */
  async createScaffold(e = {}) {
    const o = q.new();
    return { data: {
      entityType: "form",
      unique: o,
      id: o,
      created: (/* @__PURE__ */ new Date()).toJSON(),
      updated: (/* @__PURE__ */ new Date()).toJSON(),
      messageOnSubmitIsHtml: !1,
      displayDefaultFields: !0,
      daysToRetainApprovedRecordsFor: 0,
      daysToRetainRejectedRecordsFor: 0,
      daysToRetainSubmittedRecordsFor: 0,
      selectedDisplayFields: [],
      nodeId: 0,
      name: "",
      path: "",
      formWorkflows: {
        onSubmit: [],
        onApprove: [],
        onReject: []
      },
      pages: [],
      fieldIndicationType: kd.MARK_MANDATORY_FIELDS,
      indicator: "*",
      showValidationSummary: !1,
      hideFieldValidation: !1,
      requiredErrorMessage: "",
      invalidErrorMessage: "",
      messageOnSubmit: "",
      manualApproval: !1,
      storeRecordsLocally: !1,
      autocompleteAttribute: "",
      disableDefaultStylesheet: !1,
      submitLabel: "",
      nextLabel: "",
      prevLabel: "",
      showPagingOnMultiPageForms: se.NONE,
      pagingDetailsFormat: "",
      pageCaptionFormat: "",
      showSummaryPageOnMultiPageForms: !1,
      validationRules: []
    } };
  }
  /**
   * Fetches a Form with the given id from the server
   * @param {string} unique
   * @return {FormDesign}
   * @memberof FormsFormDetailServerDataSource
   */
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(d(this, Be), K.getFormById({ id: e }));
    return r || !o ? { error: r } : { data: o };
  }
  /**
   * Inserts a new Form on the server
   * @param {FormDetailModel} form
   * @return {*}
   * @memberof FormsFormDetailServerDataSource
   */
  async create(e) {
    if (!e)
      throw new Error("Form is missing");
    if (!e.unique)
      throw new Error("Form unique is missing");
    const { error: o } = await y(
      d(this, Be),
      K.postForm({
        requestBody: e
      })
    );
    return o ? { error: o } : this.read(e.unique);
  }
  /**
   * Updates a Form on the server
   * @param {FormDetailModel} Form
   * @return {*}
   * @memberof FormsFormDetailServerDataSource
   */
  async update(e) {
    if (!e.unique)
      throw new Error("Unique is missing");
    const { error: o } = await y(
      d(this, Be),
      K.putFormById({
        id: e.id,
        requestBody: e
      })
    );
    return o ? { error: o } : this.read(e.unique);
  }
  /**
   * Deletes a Form on the server
   * @param {string} unique
   * @return {*}
   * @memberof FormsFormDetailServerDataSource
   */
  async delete(e) {
    if (!e)
      throw new Error("Unique is missing");
    return y(
      d(this, Be),
      K.deleteFormById({
        id: e
      })
    );
  }
}
Be = new WeakMap();
class AS extends Se {
  constructor(e) {
    super(
      e,
      MS,
      Ip
    );
  }
  async requestRecordsMetaData(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      this._host,
      Ze.getFormByFormIdRecordMetadata({
        formId: e
      })
    );
    return r || !o ? { error: r } : { data: o };
  }
  async requestHasRelations(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      this._host,
      K.getFormByIdHasRelations({
        id: e
      })
    );
    return r || !o ? { error: r } : { data: o };
  }
  async requestTemplates() {
    const { data: e, error: o } = await y(
      this._host,
      wy.getFormTemplate()
    );
    return o || !e ? { error: o } : { data: e };
  }
  async requestFormScaffold(e) {
    const o = e.length === 0 ? y(
      this,
      K.getFormScaffold()
    ) : y(
      this,
      K.getFormScaffoldByTemplate({
        template: e
      })
    ), { data: r, error: i } = await y(this._host, o);
    return i || !r ? { error: i } : { data: r };
  }
  async copyForm(e, o, r, i) {
    if (!e)
      throw new Error("Unique is missing");
    const a = {
      newName: r,
      copyWorkflows: o,
      copyToFolderId: i
    }, { data: s, error: u } = await y(
      this._host,
      K.postFormByIdCopy({
        id: e,
        requestBody: a
      })
    );
    return u ? { error: u } : { data: s };
  }
  async copyFormWorkflows(e, o, r) {
    if (!e)
      throw new Error("sourceId is missing");
    if (!o)
      throw new Error("destinationId is missing");
    const i = {
      sourceId: e,
      destinationId: o,
      workflowIds: r
    }, { data: a, error: s } = await y(
      this._host,
      K.postFormByIdCopyWorkflows({
        id: e,
        requestBody: i
      })
    );
    return s ? { error: s } : { data: a };
  }
  async moveForm(e, o) {
    if (!e)
      throw new Error("Unique is missing");
    const r = {
      parentId: o
    }, { data: i, error: a } = await y(
      this._host,
      K.putFormByIdMove({
        id: e,
        requestBody: r
      })
    );
    return a ? { error: a } : { data: i };
  }
}
var je;
class RS {
  constructor(e) {
    f(this, je, void 0);
    _(this, je, e);
  }
  async createScaffold(e) {
    return { data: {
      entityType: he,
      unique: q.new(),
      name: "",
      ...e
    } };
  }
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, je),
      ht.getFolderById({
        id: e
      })
    );
    return o ? { data: {
      unique: o.id,
      name: o.name,
      parentUnique: o.parentId ? o.parentId : null,
      entityType: he
    } } : { error: r };
  }
  async create(e, o) {
    if (!e)
      throw new Error("Data is missing");
    if (!e.unique)
      throw new Error("Unique is missing");
    if (!e.name)
      throw new Error("Name is missing");
    const r = {
      id: e.unique,
      parent: o ? { id: o } : null,
      name: e.name
    }, { error: i } = await y(
      d(this, je),
      ht.postFolder({
        requestBody: r
      })
    );
    return i ? { error: i } : this.read(e.unique);
  }
  async update(e) {
    if (!e)
      throw new Error("Data is missing");
    if (!e.unique)
      throw new Error("Unique is missing");
    if (!e.name)
      throw new Error("Folder name is missing");
    const { error: o } = await y(
      d(this, je),
      ht.putFolderById({
        id: e.unique,
        requestBody: { name: e.name }
      })
    );
    return o ? { error: o } : this.read(e.unique);
  }
  async delete(e) {
    if (!e)
      throw new Error("Unique is missing");
    return y(
      d(this, je),
      ht.deleteFolderById({
        id: e
      })
    );
  }
}
je = new WeakMap();
class IS extends we {
  constructor(e) {
    super(
      e,
      Up.toString()
    );
  }
}
const Up = new M("FormFolderDetailStore");
class US extends Se {
  constructor(e) {
    super(e, RS, Up);
  }
  async isEmpty(e) {
    if (!e)
      throw new Error("Unique is missing");
    return y(
      this._host,
      ht.getFolderByIdIsEmpty({
        id: e
      })
    );
  }
  async moveFolder(e, o) {
    if (!e)
      throw new Error("Unique is missing");
    const r = {
      parentId: o
    }, { data: i, error: a } = await y(
      this._host,
      ht.putFolderByIdMove({
        id: e,
        requestBody: r
      })
    );
    return a || !i ? { error: a } : { data: i };
  }
}
const WS = "Forms.Repository.Form.Detail", LS = "Forms.Store.Form.Detail", Wp = "Forms.Repository.Folder.Detail", zS = "Forms.Store.Folder.Detail", NS = [
  {
    type: "repository",
    alias: WS,
    name: "Form Detail Repository",
    api: AS
  },
  {
    type: "repository",
    alias: Wp,
    name: "Form Folder Detail Repository",
    api: US
  }
], qS = [
  {
    type: "store",
    alias: LS,
    name: "Form Detail Store",
    api: xS
  },
  {
    type: "store",
    alias: zS,
    name: "Form Folder Detail Store",
    api: IS
  }
], VS = [...NS, ...qS];
class BS extends _d {
  constructor(e) {
    super(e, {
      getItems: jS,
      mapper: GS
    });
  }
}
const jS = (t) => K.getItemForm({ id: t }), GS = (t) => ({
  unique: t.id,
  name: t.name
});
class HS extends Sd {
  constructor(e) {
    super(e, Lp.toString());
  }
}
const Lp = new M("FormsFormItemStore");
class YS extends vd {
  constructor(e) {
    super(e, BS, Lp);
  }
}
class KS extends Sd {
  constructor(e) {
    super(e, zp.toString());
  }
}
const zp = new M("FormsFolderItemStore");
class XS extends _d {
  constructor(e) {
    super(e, {
      getItems: QS,
      mapper: JS
    });
  }
}
const QS = (t) => ht.getItemFolder({ id: t }), JS = (t) => ({
  unique: t.id,
  name: t.name
});
class ZS extends vd {
  constructor(e) {
    super(e, XS, zp);
  }
}
const Np = "Forms.Repository.Form.Item", qp = "Forms.Repository.Folder.Item", ew = "Forms.Store.Form.Item", tw = "Forms.Store.Folder.Item", ow = [
  {
    type: "repository",
    alias: Np,
    name: "Forms Form Item Repository",
    api: YS
  },
  {
    type: "repository",
    alias: qp,
    name: "Forms Folder Item Repository",
    api: ZS
  }
], iw = [
  {
    type: "itemStore",
    alias: ew,
    name: "Forms Form Item Store",
    api: HS
  },
  {
    type: "itemStore",
    alias: tw,
    name: "Forms Folder Item Store",
    api: KS
  }
], rw = [...ow, ...iw], aw = new $(
  "Forms.Modal.FormCreateOptions",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class sw extends X {
  async execute() {
    await (await this.getContext(D)).open(this, aw, {
      data: {
        parent: {
          entityType: this.args.entityType,
          unique: this.args.unique
        }
      }
    }).onSubmit().catch(() => {
    });
  }
}
const nw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Form.Create",
    name: "Create Form Entity Action",
    weight: 100,
    api: sw,
    forEntityTypes: [Eo, he],
    meta: {
      icon: "icon-add",
      label: "Create..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.FormCreateOptions",
    name: "Form Create Options Modal",
    js: () => import("./form-create-options-modal.element.js")
  }
], lw = [...nw], cw = new $(
  "Forms.Modal.FormCopyOptions",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class uw extends X {
  async execute() {
    await (await this.getContext(D)).open(this, cw, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const dw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Form.Copy",
    name: "Copy Form Entity Action",
    weight: 80,
    api: uw,
    forEntityTypes: [Fe],
    meta: {
      icon: "icon-documents",
      label: "Copy..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.FormCopyOptions",
    name: "Form Copy Options Modal",
    js: () => import("./form-copy-options-modal.element.js")
  }
], mw = [...dw], pw = new $(
  "Forms.Modal.FormCopyWorkflowsOptions",
  {
    modal: {
      type: "sidebar",
      size: "medium"
    }
  }
);
class hw extends X {
  async execute() {
    await (await this.getContext(D)).open(this, pw, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const fw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Form.CopyWorkflows",
    name: "Copy Form Workflows Entity Action",
    weight: 75,
    api: hw,
    forEntityTypes: [Fe],
    meta: {
      icon: "icon-documents",
      label: "Copy Workflows..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.FormCopyWorkflowsOptions",
    name: "Form Copy Workflows Options Modal",
    js: () => import("./form-copy-workflows-options-modal.element.js")
  }
], yw = [...fw], gw = new $(
  "Forms.Modal.FormDeleteConfirm",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class _w extends X {
  async execute() {
    await (await this.getContext(D)).open(this, gw, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const vw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Form.Delete",
    name: "Delete Form Entity Action",
    weight: 50,
    api: _w,
    forEntityTypes: [Fe],
    meta: {
      icon: "icon-delete",
      label: "Delete..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.FormDeleteConfirm",
    name: "Form Delete Confirm Modal",
    js: () => import("./form-delete-confirm-modal.element.js")
  }
], Sw = [...vw], ww = new $(
  "Forms.Modal.FolderDeleteConfirm",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class bw extends X {
  async execute() {
    await (await this.getContext(D)).open(this, ww, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const Fw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Folder.Delete",
    name: "Delete Folder Entity Action",
    weight: 50,
    api: bw,
    forEntityTypes: [he],
    meta: {
      icon: "icon-delete",
      label: "Delete..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.FolderDeleteConfirm",
    name: "Folder Delete Confirm Modal",
    js: () => import("./folder-delete-confirm-modal.element.js")
  }
], Ew = [...Fw], Tw = new $(
  "Forms.Modal.FormMoveOptions",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class $w extends X {
  async execute() {
    await (await this.getContext(D)).open(this, Tw, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const Cw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Form.Move",
    name: "Copy Form Entity Action",
    weight: 90,
    api: $w,
    forEntityTypes: [Fe],
    meta: {
      icon: "icon-enter",
      label: "Move..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.FormMoveOptions",
    name: "Form Move Options Modal",
    js: () => import("./form-move-options-modal.element.js")
  }
], Ow = [...Cw], Pw = new $(
  "Forms.Modal.FolderMoveOptions",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class kw extends X {
  async execute() {
    await (await this.getContext(D)).open(this, Pw, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const Dw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Folder.Move",
    name: "Copy Form Entity Action",
    weight: 90,
    api: kw,
    forEntityTypes: [he],
    meta: {
      icon: "icon-enter",
      label: "Move..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.FolderMoveOptions",
    name: "Folder Move Options Modal",
    js: () => import("./folder-move-options-modal.element.js")
  }
], xw = [...Dw], Mw = new $(
  "Forms.Modal.ExportForm",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class Aw extends X {
  async execute() {
    await (await this.getContext(D)).open(this, Mw, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const Rw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Form.Export",
    name: "Export Form Entity Action",
    weight: 60,
    api: Aw,
    forEntityTypes: [Fe],
    meta: {
      icon: "icon-download-alt",
      label: "Export Form Definition"
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.ExportForm",
    name: "Export Form Modal",
    js: () => import("./form-export-modal.element.js")
  }
], Iw = [...Rw], Uw = new $(
  "Forms.Modal.ImportForm",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class Ww extends X {
  async execute() {
    await (await this.getContext(D)).open(this, Uw, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const Lw = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Form.Import",
    name: "Import Form Entity Action",
    weight: 70,
    api: Ww,
    forEntityTypes: [
      Eo,
      he
    ],
    meta: {
      icon: "icon-page-up",
      label: "Import Form Definition"
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.ImportForm",
    name: "Import Form Modal",
    js: () => import("./form-import-modal.element.js")
  }
], zw = [...Lw], Nw = [
  {
    type: "entityAction",
    kind: "reloadTreeItemChildren",
    alias: "Forms.EntityAction.Folder.ReloadChildrenOf",
    name: "Reload Children",
    forEntityTypes: [
      he,
      Eo
    ]
  },
  {
    type: "entityAction",
    kind: "folderUpdate",
    alias: "Forms.EntityAction.Folder.Rename",
    name: "Rename Folder",
    weight: 95,
    forEntityTypes: [he],
    meta: {
      folderRepositoryAlias: Wp
    }
  }
], qw = [
  ...Nw,
  ...lw,
  ...mw,
  ...yw,
  ...Sw,
  ...Ew,
  ...Ow,
  ...xw,
  ...Iw,
  ...zw
];
var Vw = Object.defineProperty, Vp = (t, e, o, r) => {
  for (var i = void 0, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = s(e, o, i) || i);
  return i && Vw(e, o, i), i;
};
class G extends be {
  constructor() {
    super(...arguments), this.prevalues = [], this.settings = {};
  }
  getSettingValue(e) {
    return this.settings[e];
  }
}
Vp([
  p({ type: Array })
], G.prototype, "prevalues");
Vp([
  p()
], G.prototype, "settings");
var Bw = Object.defineProperty, jw = Object.getOwnPropertyDescriptor, Gw = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? jw(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Bw(e, o, i), i;
};
const Hw = "forms-field-preview-checkbox";
let ei = class extends G {
  render() {
    return n`<input type="checkbox" disabled tabindex="-1"/>`;
  }
};
ei = Gw([
  h(Hw)
], ei);
const Yw = ei, Kw = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewCheckbox() {
    return ei;
  },
  default: Yw
}, Symbol.toStringTag, { value: "Module" }));
var Xw = Object.defineProperty, Qw = Object.getOwnPropertyDescriptor, Jw = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? Qw(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Xw(e, o, i), i;
};
const Zw = "forms-field-preview-checkboxlist";
let ho = class extends G {
  render() {
    return n`
      ${Bi(
      this.prevalues.slice(0, 5),
      (t) => n`
          <div class=${this.getSettingValue("DisplayLayout") ? this.getSettingValue("DisplayLayout").toLowerCase() : ""}>
            <span>
              <input type="checkbox" disabled tabindex="-1"/>
              <label>${t.caption || t.value}</label>
            </span>
          </div>
        `
    )}
      ${m(
      this.prevalues.length > 5,
      () => n`
          <span class="ellipsis">...</span>
        `
    )}
        `;
  }
};
ho.styles = [
  fa,
  C`
      .horizontal{
        display: inline;
      }

      .horizontal .span {
        margin-right: 10px;
      }

      .ellipsis {
        margin-left: 24px;
      }
    `
];
ho = Jw([
  h(Zw)
], ho);
const eb = ho, tb = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewCheckboxList() {
    return ho;
  },
  default: eb
}, Symbol.toStringTag, { value: "Module" }));
var ob = Object.defineProperty, ib = Object.getOwnPropertyDescriptor, rb = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? ib(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && ob(e, o, i), i;
};
const ab = "forms-field-preview-dataconsent";
let ti = class extends G {
  render() {
    return n`<input type="checkbox" disabled tabindex="-1"/>
    <label>${this.getSettingValue("AcceptCopy")}</label>`;
  }
};
ti = rb([
  h(ab)
], ti);
const sb = ti, nb = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewDataConsent() {
    return ti;
  },
  default: sb
}, Symbol.toStringTag, { value: "Module" }));
var lb = Object.defineProperty, cb = Object.getOwnPropertyDescriptor, ub = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? cb(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && lb(e, o, i), i;
};
const db = "forms-field-preview-datepicker";
let oi = class extends G {
  render() {
    return n`<input type="text"
    autocomplete="off"
    disabled
    readonly
    placeholder="${this.getSettingValue("Placeholder")}"/>
    <button class="btn" disabled>
      <uui-icon name="icon-calendar"></uui-icon>
    </button>`;
  }
};
oi = ub([
  h(db)
], oi);
const mb = oi, pb = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewDatePicker() {
    return oi;
  },
  default: mb
}, Symbol.toStringTag, { value: "Module" }));
var hb = Object.defineProperty, fb = Object.getOwnPropertyDescriptor, yb = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? fb(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && hb(e, o, i), i;
};
const gb = "forms-field-preview-dropdown";
let ii = class extends G {
  render() {
    return n`<select tabindex="-1" style="opacity: 0.5; min-width: 300px">
      <option>${this.getSettingValue("SelectPrompt")}</option>
      ${Bi(
      this.prevalues,
      (t) => n`<option>${t.caption || t.value}</option>`
    )}
    </select>`;
  }
};
ii = yb([
  h(gb)
], ii);
const _b = ii, vb = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewDropdown() {
    return ii;
  },
  default: _b
}, Symbol.toStringTag, { value: "Module" }));
var Sb = Object.defineProperty, wb = Object.getOwnPropertyDescriptor, bb = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? wb(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Sb(e, o, i), i;
};
const Fb = "forms-field-preview-fileupload";
let ri = class extends G {
  render() {
    return n`<input disabled type="file" />
    <p>
      <strong>Current file(s):</strong>
    </p>`;
  }
};
ri = bb([
  h(Fb)
], ri);
const Eb = ri, Tb = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewFileUpload() {
    return ri;
  },
  default: Eb
}, Symbol.toStringTag, { value: "Module" }));
var $b = Object.defineProperty, Cb = Object.getOwnPropertyDescriptor, Ob = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? Cb(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && $b(e, o, i), i;
};
const Pb = "forms-field-preview-hiddenfield";
let ai = class extends G {
  render() {
    return n`<input disabled type="hidden" />`;
  }
};
ai = Ob([
  h(Pb)
], ai);
const kb = ai, Db = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewHiddenField() {
    return ai;
  },
  default: kb
}, Symbol.toStringTag, { value: "Module" }));
var xb = Object.defineProperty, Mb = Object.getOwnPropertyDescriptor, Ab = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? Mb(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && xb(e, o, i), i;
};
const Rb = "forms-field-preview-password";
let si = class extends G {
  render() {
    return n`<input type="password" readonly disabled tabindex="-1" placeholder=${this.getSettingValue("Placeholder")} />`;
  }
};
si = Ab([
  h(Rb)
], si);
const Ib = si, Ub = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewPassword() {
    return si;
  },
  default: Ib
}, Symbol.toStringTag, { value: "Module" }));
var Wb = Object.defineProperty, Lb = Object.getOwnPropertyDescriptor, zb = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? Lb(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Wb(e, o, i), i;
};
const Nb = "forms-field-preview-radiobuttonlist";
let fo = class extends G {
  render() {
    return n`
      ${Bi(
      this.prevalues.slice(0, 5),
      (t) => n`
          <div class=${this.getSettingValue("DisplayLayout") ? this.getSettingValue("DisplayLayout").toLowerCase() : ""}>
            <span>
              <input type="radio" disabled tabindex="-1"/>
              <label>${t.caption || t.value}</label>
            </span>
          </div>
          `
    )}
      ${m(
      this.prevalues.length > 5,
      () => n`
          <span class="ellipsis">...</span>
        `
    )}
        `;
  }
};
fo.styles = [
  fa,
  C`
      .horizontal{
        display: inline;
      }

      .horizontal .span {
        margin-right: 10px;
      }

      .ellipsis {
        margin-left: 24px;
      }
    `
];
fo = zb([
  h(Nb)
], fo);
const qb = fo, Vb = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewRadioButtonList() {
    return fo;
  },
  default: qb
}, Symbol.toStringTag, { value: "Module" }));
var Yu = Object.freeze, Bp = Object.defineProperty, Bb = Object.getOwnPropertyDescriptor, jb = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? Bb(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Bp(e, o, i), i;
}, Gb = (t, e) => Yu(Bp(t, "raw", { value: Yu(t.slice()) })), Ku;
const Hb = "forms-field-preview-recaptchav2";
let ni = class extends G {
  render() {
    return n(Ku || (Ku = Gb(['<script src="https://www.google.com/recaptcha/api.js" async defer type="application/javascript"><\/script>'])));
  }
};
ni = jb([
  h(Hb)
], ni);
const Yb = ni, Kb = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewRecaptchaV2() {
    return ni;
  },
  default: Yb
}, Symbol.toStringTag, { value: "Module" }));
var Xu = Object.freeze, jp = Object.defineProperty, Xb = Object.getOwnPropertyDescriptor, Qb = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? Xb(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && jp(e, o, i), i;
}, Jb = (t, e) => Xu(jp(t, "raw", { value: Xu(t.slice()) })), Qu;
const Zb = "forms-field-preview-recaptchav3";
let li = class extends G {
  render() {
    return n(Qu || (Qu = Jb(['<script src="https://www.google.com/recaptcha/api.js" async defer type="application/javascript"><\/script>'])));
  }
};
li = Qb([
  h(Zb)
], li);
const eF = li, tF = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewRecaptchaV3() {
    return li;
  },
  default: eF
}, Symbol.toStringTag, { value: "Module" }));
var oF = Object.defineProperty, iF = Object.getOwnPropertyDescriptor, rF = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? iF(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && oF(e, o, i), i;
};
const aF = "forms-field-preview-richtext";
let ci = class extends G {
  render() {
    return co(this.getSettingValue("Html"));
  }
};
ci = rF([
  h(aF)
], ci);
const sF = ci, nF = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewRichtext() {
    return ci;
  },
  default: sF
}, Symbol.toStringTag, { value: "Module" }));
var lF = Object.defineProperty, cF = Object.getOwnPropertyDescriptor, uF = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? cF(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && lF(e, o, i), i;
};
const dF = "forms-field-preview-titleanddescription";
let ui = class extends G {
  render() {
    return n`
    <h3>
      ${this.getSettingValue("Headline") ?? "Sample headline"}
    </h3>
    <div>
      ${this.getSettingValue("BodyText") ?? "Sample body text"}
    </div>`;
  }
};
ui = uF([
  h(dF)
], ui);
const mF = ui, pF = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewTitleAndDescription() {
    return ui;
  },
  default: mF
}, Symbol.toStringTag, { value: "Module" }));
var hF = Object.defineProperty, fF = Object.getOwnPropertyDescriptor, yF = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? fF(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && hF(e, o, i), i;
};
const gF = "forms-field-preview-text-area";
let di = class extends G {
  render() {
    return n`<textarea
      rows="5"
      readonly
      disabled
      tabindex="-1"
      placeholder=${this.getSettingValue("Placeholder")}>${this.getSettingValue("DefaultValue")}</textarea>`;
  }
};
di = yF([
  h(gF)
], di);
const _F = di, vF = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewTextArea() {
    return di;
  },
  default: _F
}, Symbol.toStringTag, { value: "Module" }));
var SF = Object.defineProperty, wF = Object.getOwnPropertyDescriptor, bF = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? wF(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && SF(e, o, i), i;
};
const FF = "forms-field-preview-text-box";
let mi = class extends G {
  render() {
    return n`<input
      type="${this.getSettingValue("FieldType") || "text"}"
      readonly
      disabled
      tabindex="-1"
      placeholder="${this.getSettingValue("Placeholder")}"
      value="${this.getSettingValue("DefaultValue")}" />`;
  }
};
mi = bF([
  h(FF)
], mi);
const EF = mi, TF = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsFieldPreviewTextBox() {
    return mi;
  },
  default: EF
}, Symbol.toStringTag, { value: "Module" })), $F = [
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.TextBox",
    name: "Text Box Field Preview",
    api: mi,
    element: () => Promise.resolve().then(() => TF)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.TextArea",
    name: "Text Area Field Preview",
    api: di,
    element: () => Promise.resolve().then(() => vF)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.Checkbox",
    name: "Checkbox Field Preview",
    api: ei,
    element: () => Promise.resolve().then(() => Kw)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.DataConsent",
    name: "Data Consent Field Preview",
    api: ti,
    element: () => Promise.resolve().then(() => nb)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.DatePicker",
    name: "Date Picker Field Preview",
    api: oi,
    element: () => Promise.resolve().then(() => pb)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.Dropdown",
    name: "Dropdown Field Preview",
    api: ii,
    element: () => Promise.resolve().then(() => vb)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.FileUpload",
    name: "File Upload Field Preview",
    api: ri,
    element: () => Promise.resolve().then(() => Tb)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.HiddenField",
    name: "Hidden Field Preview",
    api: ai,
    element: () => Promise.resolve().then(() => Db)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.PasswordField",
    name: "Password Field Preview",
    api: si,
    element: () => Promise.resolve().then(() => Ub)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.RadioButtonList",
    name: "Radio Button List Field Preview",
    api: fo,
    element: () => Promise.resolve().then(() => Vb)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.CheckboxList",
    name: "Checkbox List Field Preview",
    api: ho,
    element: () => Promise.resolve().then(() => tb)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.RecaptchaV2",
    name: "Recaptcha V2 Field Preview",
    api: ni,
    element: () => Promise.resolve().then(() => Kb)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.RecaptchaV3",
    name: "Recaptcha V3 Field Preview",
    api: li,
    element: () => Promise.resolve().then(() => tF)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.TitleAndDescription",
    name: "Title And Description Field Preview",
    api: ui,
    element: () => Promise.resolve().then(() => pF)
  },
  {
    type: "formsFieldPreview",
    alias: "Forms.FieldPreview.Richtext",
    name: "Richtext Field Preview",
    api: ci,
    element: () => Promise.resolve().then(() => nF)
  }
], CF = [...VS, ...rw];
var io, bt;
class OF extends bd {
  constructor(o) {
    super(o);
    f(this, io, void 0);
    f(this, bt, void 0);
    _(this, io, o);
  }
  async getCollection(o) {
    const r = await this.getContext(Ae);
    if (_(this, bt, await Kl(r.unique.pipe(Xf((b) => !!b)))), !d(this, bt))
      return { data: { total: 0, items: [] } };
    if (location.href.includes(
      `modal/${oy(Ag.toString())}`
    )) {
      const b = location.href.split("/").pop(), I = await Ze.getFormByFormIdRecordPageNumber({
        recordId: b,
        skip: 0,
        take: o.take,
        formId: d(this, bt),
        sortBy: "created",
        sortOrder: Zl.DESCENDING
      }) ?? 1;
      o = {
        ...o,
        skip: (I - 1) * o.take
      }, (await this.getContext(Da)).pagination.setCurrentPageNumber(I);
    }
    o = { ...o, formId: d(this, bt) };
    const { data: i, error: a } = await y(
      d(this, io),
      Ze.getFormByFormIdRecord(o)
    );
    if (a)
      return { error: a };
    if (!i)
      return { data: { items: [], total: 0 } };
    const { schema: s, results: u, totalNumberOfResults: S } = i, g = u.map(
      (b) => {
        var B, Y;
        return {
          unique: b.uniqueId,
          entityType: rs,
          id: b.id,
          created: new Date(b.created),
          updated: new Date(b.updated),
          state: b.state,
          pageName: (B = b.umbracoPage) == null ? void 0 : B.name,
          documentUnique: (Y = b.umbracoPage) == null ? void 0 : Y.unique,
          fields: b.fields,
          member: b.member
        };
      }
    );
    return g.unshift({
      entityType: rs,
      schema: s,
      unique: "",
      fields: [],
      id: 0,
      created: /* @__PURE__ */ new Date(),
      updated: /* @__PURE__ */ new Date(),
      state: "",
      pageName: "",
      documentUnique: ""
    }), { data: { items: g, total: S } };
  }
  async execute(o, r, i) {
    const a = { recordKeys: r };
    return y(
      d(this, io),
      Ze.postFormByFormIdRecordActionsByActionIdExecute({
        formId: o,
        actionId: i,
        requestBody: a
      })
    );
  }
}
io = new WeakMap(), bt = new WeakMap();
var ro;
class Tn {
  constructor(e) {
    f(this, ro, void 0);
    _(this, ro, new OF(e));
  }
  async requestCollection(e = {
    skip: 0,
    take: 10,
    formId: ""
  }) {
    return e.sortBy || (e = { ...e, sortBy: "created", sortOrder: Zl.DESCENDING }), d(this, ro).getCollection(e);
  }
  async execute(e, o, r) {
    return d(this, ro).execute(e, o, r);
  }
  destroy() {
  }
}
ro = new WeakMap();
const PF = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  FormsFormEntryCollectionRepository: Tn,
  default: Tn
}, Symbol.toStringTag, { value: "Module" }));
var ao;
class kF {
  constructor(e) {
    f(this, ao, void 0);
    _(this, ao, e);
  }
  async getCollection(e) {
    const { data: o, error: r } = await y(
      d(this, ao),
      Ze.getFormByFormIdRecordByRecordIdWorkflowAuditTrail(
        e
      )
    );
    if (r)
      return { error: r };
    if (!o)
      return { data: { items: [], total: 0 } };
    const i = o.map(
      (a) => ({
        entityType: "forms-record-workflow-audit-entry",
        unique: a.id.toString(),
        workflowKey: a.workflowKey,
        name: a.name,
        typeName: a.typeName,
        executedOn: new Date(a.executedOn),
        executionStage: a.executionStage,
        result: a.result
      })
    );
    return { data: { items: i, total: i.length } };
  }
  async executeWorkflow(e, o, r) {
    await y(
      d(this, ao),
      Ze.postFormByFormIdRecordByRecordIdWorkflowByWorkflowIdRetry({
        formId: e,
        recordId: o,
        workflowId: r
      })
    );
  }
}
ao = new WeakMap();
var so;
class Ju {
  constructor(e) {
    f(this, so, void 0);
    _(this, so, new kF(e));
  }
  async requestCollection(e = {
    formId: "",
    recordId: ""
  }) {
    return d(this, so).getCollection(e);
  }
  async executeWorkflow(e, o, r) {
    await d(this, so).executeWorkflow(e, o, r);
  }
  destroy() {
  }
}
so = new WeakMap();
const DF = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  FormsFormEntryWorkflowAuditCollectionRepository: Ju,
  default: Ju
}, Symbol.toStringTag, { value: "Module" }));
var Di;
class xF {
  constructor(e) {
    f(this, Di, void 0);
    _(this, Di, e);
  }
  async getCollection(e) {
    const { data: o, error: r } = await y(
      d(this, Di),
      Ze.getFormByFormIdRecordByRecordIdAuditTrail(
        e
      )
    );
    if (r)
      return { error: r };
    if (!o)
      return { data: { items: [], total: 0 } };
    const i = o.map(
      (a) => ({
        entityType: "forms-record-audit-entry",
        unique: a.id.toString(),
        updatedBy: a.updatedBy,
        updatedOn: new Date(a.updatedOn)
      })
    );
    return { data: { items: i, total: i.length } };
  }
}
Di = new WeakMap();
var xi;
class Zu {
  constructor(e) {
    f(this, xi, void 0);
    _(this, xi, new xF(e));
  }
  async requestCollection(e = {
    formId: "",
    recordId: ""
  }) {
    return d(this, xi).getCollection(e);
  }
  destroy() {
  }
}
xi = new WeakMap();
const MF = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  FormsFormEntryRecordAuditCollectionRepository: Zu,
  default: Zu
}, Symbol.toStringTag, { value: "Module" })), Gp = "Forms.Repository.Entry.Collection", AF = "Forms.Repository.EntryWorkflowAudit.Collection", RF = "Forms.Repository.EntryRecordAudit.Collection", IF = [
  {
    type: "repository",
    alias: Gp,
    name: "Entry Collection Repository",
    api: () => Promise.resolve().then(() => PF)
  },
  {
    type: "repository",
    alias: AF,
    name: "Entry Workflow Audit Collection Repository",
    api: () => Promise.resolve().then(() => DF)
  },
  {
    type: "repository",
    alias: RF,
    name: "Entry Record Audit Collection Repository",
    api: () => Promise.resolve().then(() => MF)
  }
], UF = [...IF], Hp = "Forms.CollectionView.Entry.Table", WF = {
  type: "collectionView",
  alias: Hp,
  name: "Form Entries Table Collection View",
  js: () => import("./form-entry-table-collection-view.element.js"),
  meta: {
    label: "Table",
    icon: "icon-list",
    pathName: "table"
  },
  conditions: [
    {
      alias: Xl,
      match: "Forms.Collection.Entry"
    }
  ]
}, LF = [WF];
class zF extends bd {
  async execute() {
    const e = await this.getContext(
      Ae
    );
    await (await this.getContext(D)).open(this, Ig, {
      data: {
        unique: e.getUnique()
      }
    }).onSubmit().catch(() => {
    });
  }
}
const NF = {
  type: "collectionAction",
  kind: "button",
  name: "Export Records Collection Action",
  alias: "Forms.CollectionAction.Records.Export",
  api: zF,
  weight: 200,
  meta: {
    label: "Export"
  },
  conditions: [
    {
      alias: Xl,
      match: "Forms.Collection.Entry"
    }
  ]
}, qF = [NF];
var Mi, $n;
class VF extends ey {
  constructor(o) {
    super(o, Hp);
    f(this, Mi);
  }
  oneMonthAgo() {
    const o = /* @__PURE__ */ new Date(), r = new Date(o.setMonth(o.getMonth() - 1));
    return F(this, Mi, $n).call(this, r);
  }
  today() {
    return F(this, Mi, $n).call(this, /* @__PURE__ */ new Date());
  }
}
Mi = new WeakSet(), $n = function(o) {
  const r = String(o.getDate()).padStart(2, "0"), i = String(o.getMonth() + 1).padStart(2, "0");
  return o.getFullYear() + "-" + i + "-" + r;
};
const Yp = "Forms.Collection.Entry", BF = {
  type: "collection",
  alias: Yp,
  name: "Form Entries Collection",
  api: VF,
  element: () => import("./form-entry-collection.element.js"),
  meta: {
    repositoryAlias: Gp
  }
}, jF = [
  BF,
  ...LF,
  ...UF,
  ...qF
];
var Ai, Ri, Ii;
class Ac extends iy {
  constructor(o, r, i) {
    super(o, r);
    f(this, Ai, void 0);
    f(this, Ri, void 0);
    f(this, Ii, "");
    this.consumeContext(Ae, async (a) => {
      _(this, Ai, a);
    }), this.consumeContext(ty, (a) => {
      _(this, Ri, a);
    }), _(this, Ii, i);
  }
  async execute() {
    await new Tn(this._host).execute(d(this, Ai).getUnique(), this.selection, d(this, Ii)), d(this, Ri).requestCollection();
  }
}
Ai = new WeakMap(), Ri = new WeakMap(), Ii = new WeakMap();
const GF = "cb126b70-9011-11df-a4ee-0800200c9a66";
class HF extends Ac {
  constructor(e, o) {
    super(e, o, GF);
  }
}
const YF = "cb126b79-9011-11df-a4ee-0800200c9a66";
class KF extends Ac {
  constructor(e, o) {
    super(e, o, YF);
  }
}
const XF = "84cd75a7-d3d9-4551-9c1a-3f478b4ec9ed";
class QF extends Ac {
  constructor(e, o) {
    super(e, o, XF);
  }
}
const JF = [
  {
    name: "Approve",
    weight: 100,
    api: KF,
    conditions: [
      {
        alias: En,
        match: (t) => t.manualApproval
      }
    ]
  },
  {
    name: "Reject",
    weight: 90,
    api: QF,
    conditions: [
      {
        alias: En,
        match: (t) => t.manualApproval
      }
    ]
  },
  {
    name: "Delete",
    weight: 10,
    api: HF,
    conditions: [
      {
        alias: ce,
        match: (t) => t.userSecurity.deleteEntries
      }
    ]
  }
], ZF = JF.map(
  (t) => ({
    type: "entityBulkAction",
    alias: `Forms.EntityBulkAction.Entry.${t.name}`,
    name: `${t.name} Form Entry Bulk Action`,
    weight: t.weight,
    api: t.api,
    meta: {
      label: t.name
    },
    forEntityTypes: [rs],
    conditions: [
      {
        alias: Xl,
        match: Yp
      },
      ...t.conditions ?? []
    ]
  })
), eE = [...ZF], Rc = "Forms.Condition.SecurityOption", pt = "Forms.Workspace.Form", tE = {
  type: "workspace",
  kind: "routable",
  alias: pt,
  name: "Form Workspace",
  api: () => import("./form-workspace.context.js"),
  meta: {
    entityType: Fe
  }
}, oE = [
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.Form.Design",
    name: "Form Workspace Design View",
    element: () => import("./workspace-view-form-design.element.js"),
    weight: 50,
    meta: {
      label: "Design",
      pathname: "design",
      icon: "document"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: pt
      },
      {
        alias: ce,
        match: (t) => t.userSecurity.manageForms
      }
    ]
  },
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.Form.Settings",
    name: "Form Workspace Settings View",
    element: () => import("./workspace-view-form-settings.element.js"),
    weight: 40,
    meta: {
      label: "Settings",
      pathname: "settings",
      icon: "settings"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: pt
      },
      {
        alias: ce,
        match: (t) => t.userSecurity.manageForms
      }
    ]
  },
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.Form.Advanced",
    name: "Form Workspace Advanced View",
    element: () => import("./workspace-view-form-advanced.element.js"),
    weight: 30,
    meta: {
      label: "Advanced",
      pathname: "advanced",
      icon: "code"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: pt
      },
      {
        alias: ce,
        match: (t) => t.userSecurity.manageForms
      },
      {
        alias: Rc,
        match: (t) => t.enableAdvancedValidationRules
      }
      // This is necessary as ConditionType is defined in core, and we don't have the ability to extend it.
    ]
  },
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.Form.Entries",
    name: "Form Workspace Entries View",
    element: () => import("./workspace-view-form-entries.element.js"),
    weight: 20,
    meta: {
      label: "Entries",
      pathname: "entries",
      icon: "icon-categories"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: pt
      },
      {
        alias: ce,
        match: (t) => t.userSecurity.viewEntries
      }
    ]
  },
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.Form.Info",
    name: "Form Workspace Info View",
    element: () => import("./workspace-view-form-info.element.js"),
    weight: 10,
    meta: {
      label: "Info",
      pathname: "info",
      icon: "info"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: pt
      }
    ]
  }
], iE = [
  {
    type: "workspaceAction",
    kind: "default",
    alias: "Forms.WorkspaceAction.Form.Save",
    name: "Save Form Workspace Action",
    api: Bo,
    meta: {
      label: "Save",
      look: "primary",
      color: "positive"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: pt
      }
    ]
  }
], rE = [
  tE,
  ...oE,
  ...iE,
  ...eE
], aE = [...jF], sE = [
  ...DS,
  ...qw,
  ...$F,
  ...$g,
  ...CF,
  ...rE,
  ...aE
], $o = "prevaluesource", tr = "prevaluesource-root";
class nE extends ma {
  constructor(e) {
    super(e, {
      getRootItems: Kp,
      getChildrenOf: lE,
      getAncestorsOf: cE,
      mapper: uE
    });
  }
}
const Kp = (t) => ft.getTreePrevalueSourceRoot(), lE = (t) => {
  if (t.parent.unique === null)
    return Kp();
  throw new Error("Not supported for the data source tree");
}, cE = (t) => {
  throw new Error("Not supported for the data source tree");
}, uE = (t) => ({
  unique: t.id,
  parent: {
    unique: null,
    entityType: tr
  },
  name: t.name,
  entityType: $o,
  isFolder: t.isFolder,
  hasChildren: t.hasChildren
});
class dE extends pa {
  constructor(e) {
    super(e, Xp.toString());
  }
}
const Xp = new M(
  "FormsPrevalueSourceTreeStore"
);
class mE extends ha {
  constructor(e) {
    super(e, nE, Xp);
  }
  async requestTreeRoot() {
    return { data: {
      unique: null,
      entityType: tr,
      name: "Prevalue Sources",
      hasChildren: !0,
      isFolder: !0
    } };
  }
}
const pE = {
  type: "menuItem",
  kind: "tree",
  alias: "Forms.MenuItem.PrevalueSource",
  name: "Forms Prevalue Source Menu Item",
  weight: 200,
  meta: {
    label: "Prevalue Sources",
    entityType: $o,
    treeAlias: "Forms.Tree.PrevalueSources",
    menus: [kt]
  }
}, hE = [pE], Qp = "Forms.Repository.PrevalueSources.Tree", fE = "Forms.Store.PrevalueSources.Tree", yE = "Forms.Tree.PrevalueSources", gE = {
  type: "repository",
  alias: Qp,
  name: "Prevalue Source Tree Repository",
  api: mE
}, _E = {
  type: "treeStore",
  alias: fE,
  name: "Prevalue Source Tree Store",
  api: dE
}, vE = {
  type: "tree",
  kind: "default",
  alias: yE,
  name: "Prevalue Source Tree",
  meta: {
    repositoryAlias: Qp
  },
  conditions: [
    {
      alias: ce,
      match: (t) => t.userSecurity.managePreValueSources
    }
  ]
}, SE = {
  type: "treeItem",
  kind: "default",
  alias: "Forms.TreeItem.PrevalueSource",
  name: "Prevalue Source Tree Item",
  forEntityTypes: [tr, $o]
}, wE = [
  gE,
  _E,
  vE,
  SE,
  ...hE
], bE = "Forms.Repository.PrevalueSourceCollection", FE = {
  type: "repository",
  alias: bE,
  name: "Prevalue Source Collection Repository",
  api: () => import("./prevaluesource-collection.repository.js")
}, EE = [FE];
class TE extends we {
  constructor(e) {
    super(
      e,
      Jp.toString()
    );
  }
}
const Jp = new M("PrevalueSourceDetailStore");
var Ge;
class $E {
  constructor(e) {
    f(this, Ge, void 0);
    _(this, Ge, e);
  }
  /**
   * Creates a new prevalue source scaffold
   * @param {(string | null)} parentUnique
   * @return { FieldPreValueSource }
   * @memberof FormsPrevalueSourceDetailServerDataSource
   */
  async createScaffold(e = {}) {
    return { data: {
      entityType: $o,
      unique: q.new(),
      id: q.new(),
      created: "",
      name: "",
      settings: {},
      fieldPreValueSourceTypeId: "",
      cachePrevaluesFor: "",
      updated: ""
    } };
  }
  /**
   * Fetches a prevalue source with the given id from the server
   * @param {string} unique
   * @return {FieldPreValueSource}
   * @memberof FormsPrevalueSourceDetailServerDataSource
   */
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, Ge),
      ft.getPrevalueSourceById({
        id: e
      })
    );
    return r || !o ? { error: r } : { data: o };
  }
  /**
   * Inserts a new prevalue source on the server
   * @param {FormDetailModel} form
   * @return {*}
   * @memberof FormsPrevalueSourceDetailServerDataSource
   */
  async create(e) {
    if (!e)
      throw new Error("Prevalue source is missing");
    if (!e.unique)
      throw new Error("Prevalue source unique is missing");
    const { error: o } = await y(
      d(this, Ge),
      ft.postPrevalueSource({
        requestBody: e
      })
    );
    return o ? { error: o } : this.read(e.unique);
  }
  /**
   * Updates a prevalue source on the server
   * @param {FormDetailModel} Form
   * @return {*}
   * @memberof FormsPrevalueSourceDetailServerDataSource
   */
  async update(e) {
    if (!e.unique)
      throw new Error("Unique is missing");
    const { error: o } = await y(
      d(this, Ge),
      ft.putPrevalueSourceById({
        id: e.id,
        requestBody: e
      })
    );
    return o ? { error: o } : this.read(e.unique);
  }
  /**
   * Deletes a prevalue source on the server
   * @param {string} unique
   * @return {*}
   * @memberof FormsPrevalueSourceDetailServerDataSource
   */
  async delete(e) {
    if (!e)
      throw new Error("Unique is missing");
    return y(
      d(this, Ge),
      ft.deletePrevalueSourceById({
        id: e
      })
    );
  }
}
Ge = new WeakMap();
class Zp extends Se {
  constructor(e) {
    super(
      e,
      $E,
      Jp
    );
  }
  async requestPrevalueSourceScaffold() {
    const { data: e, error: o } = await y(this._host, ft.getPrevalueSourceScaffold());
    return o || !e ? { error: o } : { data: e };
  }
  async requestPrevalues(e, o, r) {
    const { data: i, error: a } = await y(this._host, ft.getPrevalueSourceByIdValues({ id: e, formId: o, fieldId: r }));
    return a || !i ? { error: a } : { data: i };
  }
}
const CE = "Forms.Repository.PrevalueSource.Detail", OE = "Forms.Store.PrevalueSource.Detail", PE = {
  type: "repository",
  alias: CE,
  name: "Prevalue Source Detail Repository",
  api: Zp
}, kE = {
  type: "store",
  alias: OE,
  name: "PRevalue Source Detail Store",
  api: TE
}, DE = [PE, kE], xE = [...EE, ...DE], ME = new $(
  "Forms.Modal.PrevalueSourceCreateOptions",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class AE extends X {
  async execute() {
    await (await this.getContext(D)).open(this, ME, {
      data: {}
    }).onSubmit().catch(() => {
    });
  }
}
const RE = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.PrevalueSource.Create",
    name: "Create Prevalue Source Entity Action",
    weight: 1e3,
    api: AE,
    forEntityTypes: [tr],
    meta: {
      icon: "icon-add",
      label: "Create..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.PrevalueSourceCreateOptions",
    name: "Prevalue Source Create Options Modal",
    js: () => import("./prevaluesource-create-options-modal.element.js")
  }
], IE = [...RE], UE = new $(
  "Forms.Modal.PrevalueSourceDeleteConfirm",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class WE extends X {
  async execute() {
    await (await this.getContext(D)).open(this, UE, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const LE = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.PrevalueSource.Delete",
    name: "Delete Form Entity Action",
    weight: 100,
    api: WE,
    forEntityTypes: [$o],
    meta: {
      icon: "icon-delete",
      label: "Delete..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.PrevalueSourceDeleteConfirm",
    name: "Prevalue Source Delete Confirm Modal",
    js: () => import("./prevaluesource-delete-confirm-modal.element.js")
  }
], zE = [...LE], NE = [
  {
    type: "entityAction",
    kind: "reloadTreeItemChildren",
    alias: "Forms.EntityAction.PrevalueSource.ReloadChildrenOf",
    name: "Reload Children",
    forEntityTypes: [tr]
  }
], qE = [...NE, ...IE, ...zE], Nr = "Forms.Workspace.PrevalueSource", VE = {
  type: "workspace",
  kind: "routable",
  alias: Nr,
  name: "Prevalue Source Workspace",
  api: () => import("./prevaluesource-workspace.context.js"),
  meta: {
    entityType: $o
  }
}, BE = [
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.PrevalueSource.Design",
    name: "Form Workspace Design View",
    element: () => import("./workspace-view-prevaluesource-design.element.js"),
    weight: 90,
    meta: {
      label: "Design",
      pathname: "design",
      icon: "document"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Nr
      },
      {
        alias: ce,
        match: (t) => t.userSecurity.managePreValueSources
      }
    ]
  },
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.PrevalueSource.Info",
    name: "Form Workspace Info View",
    element: () => import("./workspace-view-prevaluesource-info.element.js"),
    weight: 90,
    meta: {
      label: "Info",
      pathname: "info",
      icon: "info"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Nr
      },
      {
        alias: ce,
        match: (t) => t.userSecurity.managePreValueSources
      }
    ]
  }
], jE = [
  {
    type: "workspaceAction",
    kind: "default",
    alias: "Forms.WorkspaceAction.PrevalueSource.Save",
    name: "Save Prevalue Source Workspace Action",
    api: Bo,
    meta: {
      label: "Save",
      look: "primary",
      color: "positive"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Nr
      }
    ]
  }
], GE = [VE, ...BE, ...jE], HE = [
  ...wE,
  ...xE,
  ...qE,
  ...GE
];
class YE extends $d {
  constructor(e, o) {
    super(e, o), this.consumeContext(Qi, (r) => {
      this.observe(r.userSecurity, (i) => {
        i && (this.permitted = this.config.match(i));
      });
    });
  }
}
const KE = [
  {
    type: "condition",
    name: "Form Workspace View Condition",
    alias: ce,
    api: YE
  },
  {
    type: "condition",
    name: "Security Options  Condition",
    alias: Rc,
    api: () => import("./security-options.condition.js")
  }
], XE = [...KE], Co = "forms-security-user", or = "forms-security-user-group", xa = "forms-security-user-folder", Ic = "forms-security-user-group-folder", Uc = "forms-security-root", QE = new $(
  "Forms.Modal.UserSecurityCreateOptions",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class JE extends X {
  async execute() {
    await (await this.getContext(D)).open(this, QE, {
      data: {}
    }).onSubmit().catch(() => {
    });
  }
}
const ZE = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Security.User.Create",
    name: "Create User Security Record Entity Action",
    weight: 1e3,
    api: JE,
    forEntityTypes: [xa],
    meta: {
      icon: "icon-add",
      label: "Create..."
    }
  },
  {
    type: "modal",
    alias: "Forms.Modal.UserSecurityCreateOptions",
    name: "User Security Create Options Modal",
    js: () => import("./user-security-create-options-modal.element.js")
  }
], eT = [...ZE], tT = new $(
  "Forms.Modal.UserSecurityDeleteConfirm",
  {
    modal: {
      type: "sidebar",
      size: "small"
    }
  }
);
class oT extends X {
  async execute() {
    await (await this.getContext(D)).open(this, tT, {
      data: {
        unique: this.args.unique
      }
    }).onSubmit().catch(() => {
    });
  }
}
const iT = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Forms.EntityAction.Security.User.Delete",
    name: "Delete Form Entity Action",
    weight: 100,
    api: oT,
    forEntityTypes: [Co],
    meta: {
      icon: "icon-delete",
      label: "Delete..."
    },
    conditions: [
      {
        alias: Rc,
        match: (t) => t.manageSecurityWithUserGroups
      }
      // This is necessary as ConditionType is defined in core, and we don't have the ability to extend it.
    ]
  },
  {
    type: "modal",
    alias: "Forms.Modal.UserSecurityDeleteConfirm",
    name: "User Security Delete Confirm Modal",
    js: () => import("./user-security-delete-confirm-modal.element.js")
  }
], rT = [...iT], aT = [
  {
    type: "entityAction",
    kind: "reloadTreeItemChildren",
    alias: "Forms.EntityAction.Security.ReloadChildrenOf",
    name: "Reload Children",
    forEntityTypes: [Uc, xa, Ic]
  }
], sT = [...aT, ...eT, ...rT];
class nT extends we {
  constructor(e) {
    super(
      e,
      eh.toString()
    );
  }
}
const eh = new M("FormsSecurityUserDetailStore");
var He;
class lT {
  constructor(e) {
    f(this, He, void 0);
    _(this, He, e);
  }
  async createScaffold(e = {}) {
    const o = e.unique ? e.unique : q.new();
    return { data: {
      entityType: Co,
      unique: o,
      name: "",
      key: o,
      userSecurity: {
        manageDataSources: !1,
        managePreValueSources: !1,
        manageWorkflows: !1,
        manageForms: !1,
        viewEntries: !1,
        editEntries: !1,
        deleteEntries: !1,
        user: ""
      },
      startFolderIds: [],
      formsSecurity: []
    } };
  }
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, He),
      ie.getSecurityUserByIdFormSecurity(
        { id: e }
      )
    );
    return r || !o ? { error: r } : { data: o };
  }
  async create(e) {
    if (!e)
      throw new Error("Security item is missing");
    if (!e.unique)
      throw new Error("Security item unique is missing");
    const { error: o } = await y(
      d(this, He),
      ie.postSecurityUserByIdFormSecurity(
        {
          id: e.unique,
          requestBody: e
        }
      )
    );
    return o ? { error: o } : this.read(e.unique);
  }
  async update(e) {
    if (!e.unique)
      throw new Error("Unique is missing");
    const { error: o } = await y(
      d(this, He),
      ie.putSecurityUserByIdFormSecurity(
        {
          id: e.unique,
          requestBody: e
        }
      )
    );
    return o ? { error: o } : this.read(e.unique);
  }
  async delete(e) {
    if (!e)
      throw new Error("Unique is missing");
    return y(
      d(this, He),
      ie.deleteSecurityUserByIdFormSecurity(
        {
          id: e
        }
      )
    );
  }
}
He = new WeakMap();
class cT extends Se {
  constructor(e) {
    super(
      e,
      lT,
      eh
    );
  }
  async requestUsersToAssign() {
    return await ie.getSecurityUserUsersToAssign();
  }
}
const uT = "Forms.Repository.Security.User.Detail", dT = "Forms.Store.Security.User.Detail", mT = {
  type: "repository",
  alias: uT,
  name: "Form User Source Detail Repository",
  api: cT
}, pT = {
  type: "store",
  alias: dT,
  name: "Form User Security Detail Store",
  api: nT
}, hT = [mT, pT];
class fT extends we {
  constructor(e) {
    super(
      e,
      th.toString()
    );
  }
}
const th = new M("FormsSecurityUserGroupDetailStore");
var Ye;
class yT {
  constructor(e) {
    f(this, Ye, void 0);
    _(this, Ye, e);
  }
  async createScaffold(e = {}) {
    return { data: {
      entityType: or,
      unique: q.new(),
      name: "",
      key: "",
      userGroupSecurity: {
        manageDataSources: !1,
        managePreValueSources: !1,
        manageWorkflows: !1,
        manageForms: !1,
        viewEntries: !1,
        editEntries: !1,
        deleteEntries: !1,
        userGroupId: 0
      },
      startFolderIds: [],
      formsSecurity: []
    } };
  }
  async read(e) {
    if (!e)
      throw new Error("Unique is missing");
    const { data: o, error: r } = await y(
      d(this, Ye),
      ie.getSecurityUserGroupByIdFormSecurity(
        { id: e }
      )
    );
    return r || !o ? { error: r } : { data: o };
  }
  async create(e) {
    if (!e)
      throw new Error("Security item is missing");
    if (!e.unique)
      throw new Error("Security item unique is missing");
    const { error: o } = await y(
      d(this, Ye),
      ie.postSecurityUserGroupByIdFormSecurity(
        {
          id: e.unique,
          requestBody: e
        }
      )
    );
    return o ? { error: o } : this.read(e.unique);
  }
  async update(e) {
    if (!e.unique)
      throw new Error("Unique is missing");
    const { error: o } = await y(
      d(this, Ye),
      ie.putSecurityUserGroupByIdFormSecurity(
        {
          id: e.unique,
          requestBody: e
        }
      )
    );
    return o ? { error: o } : this.read(e.unique);
  }
  async delete(e) {
    if (!e)
      throw new Error("Unique is missing");
    return y(
      d(this, Ye),
      ie.deleteSecurityUserGroupByIdFormSecurity(
        {
          id: e
        }
      )
    );
  }
}
Ye = new WeakMap();
class gT extends Se {
  constructor(e) {
    super(
      e,
      yT,
      th
    );
  }
}
const _T = "Forms.Repository.Security.UserGroup.Detail", vT = "Forms.Store.Security.UserGroup.Detail", ST = {
  type: "repository",
  alias: _T,
  name: "Form User Group Source Detail Repository",
  api: gT
}, wT = {
  type: "store",
  alias: vT,
  name: "Form User Group Security Detail Store",
  api: fT
}, bT = [ST, wT], FT = [...hT, ...bT];
class ET extends ma {
  constructor(e) {
    super(e, {
      getRootItems: oh,
      getChildrenOf: TT,
      getAncestorsOf: $T,
      mapper: CT
    });
  }
}
const oh = (t) => ie.getTreeSecurityRoot(), TT = (t) => t.parent.unique === null ? oh() : ie.getTreeSecurityChildrenByParentId({
  parentId: t.parent.unique
}), $T = (t) => {
  throw new Error("Not supported for the security tree");
}, CT = (t) => {
  var e;
  return {
    unique: t.id,
    parent: { unique: ((e = t.parent) == null ? void 0 : e.id) || "", entityType: "" },
    name: t.name,
    entityType: t.isFolder ? t.id === "207c2294-970b-4e1f-82fd-ae8996ef171d" ? xa : Ic : t.isGroup ? or : Co,
    isFolder: t.isFolder,
    hasChildren: t.hasChildren
  };
};
class OT extends pa {
  constructor(e) {
    super(e, ih.toString());
  }
}
const ih = new M(
  "FormsSecurityTreeStore"
);
class PT extends ha {
  constructor(e) {
    super(e, ET, ih);
  }
  async requestTreeRoot() {
    return { data: {
      unique: null,
      entityType: Uc,
      name: "Security",
      hasChildren: !0,
      isFolder: !0
    } };
  }
}
const kT = [
  {
    type: "menuItem",
    kind: "tree",
    alias: "Forms.MenuItem.UserSecurity",
    name: "Forms Security User Menu Item",
    weight: 200,
    meta: {
      label: "User Security",
      entityType: Co,
      treeAlias: "Forms.Tree.Security",
      menus: [kt]
    }
  },
  {
    type: "menuItem",
    kind: "tree",
    alias: "Forms.MenuItem.UserGroupSecurity",
    name: "Forms Security User Group Menu Item",
    weight: 200,
    meta: {
      label: "User Group Security",
      entityType: or,
      treeAlias: "Forms.Tree.GroupSecurity",
      menus: [kt]
    }
  }
], DT = [...kT], rh = "Forms.Repository.Security.Tree", xT = "Forms.Store.Security.Tree", MT = "Forms.Tree.Security", AT = {
  type: "repository",
  alias: rh,
  name: "Forms Security Tree Repository",
  api: PT
}, RT = {
  type: "treeStore",
  alias: xT,
  name: "Forms Security Tree Store",
  api: OT
}, IT = {
  type: "tree",
  kind: "default",
  alias: MT,
  name: "Forms Security Tree",
  meta: {
    repositoryAlias: rh
  },
  conditions: [
    {
      alias: "Umb.Condition.SectionUserPermission",
      match: "Umb.Section.Users"
    }
  ]
}, UT = {
  type: "treeItem",
  kind: "default",
  alias: "Forms.TreeItem.Security",
  name: "Forms Security Tree Item",
  forEntityTypes: [Co, or, xa, Ic, Uc]
}, WT = [
  AT,
  RT,
  IT,
  UT,
  ...DT
], Wc = "Forms.Workspace.Security.User", Lc = "Forms.Workspace.Security.UserGroup", LT = [
  {
    type: "workspace",
    kind: "routable",
    alias: Wc,
    name: "Forms User Security Workspace",
    api: () => import("./security-user-workspace.context.js"),
    meta: {
      entityType: Co
    }
  },
  {
    type: "workspace",
    kind: "routable",
    alias: Lc,
    name: "Forms User Group Security Workspace",
    api: () => import("./security-user-group-workspace.context.js"),
    meta: {
      entityType: or
    }
  }
], zT = [
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.Security.User.Permissions",
    name: "Security Workspace User Permissions View",
    element: () => import("./workspace-view-security-user-permissions.element.js"),
    weight: 90,
    meta: {
      label: "Permissions",
      pathname: "permissions",
      icon: "user"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Wc
      }
      // TODO: user permissions for access to manage form security
    ]
  },
  {
    type: "workspaceView",
    alias: "Forms.WorkspaceView.Security.UserGroup.Permissions",
    name: "Security Workspace User Group Permissions View",
    element: () => import("./workspace-view-security-user-group-permissions.element.js"),
    weight: 90,
    meta: {
      label: "Permissions",
      pathname: "permissions",
      icon: "user"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Lc
      }
      // TODO: user permissions for access to manage form security
    ]
  }
], NT = [
  {
    type: "workspaceAction",
    kind: "default",
    alias: "Forms.WorkspaceAction.Security.User.Save",
    name: "Save Security Workspace Action",
    api: Bo,
    meta: {
      label: "Save",
      look: "primary",
      color: "positive"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Wc
      }
    ]
  },
  {
    type: "workspaceAction",
    kind: "default",
    alias: "Forms.WorkspaceAction.Security.UserGroup.Save",
    name: "Save Security Workspace Action",
    api: Bo,
    meta: {
      label: "Save",
      look: "primary",
      color: "positive"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: Lc
      }
    ]
  }
], qT = [...LT, ...zT, ...NT], VT = [
  ...XE,
  ...sT,
  ...FT,
  ...WT,
  ...qT
];
var BT = Object.defineProperty, jT = Object.getOwnPropertyDescriptor, GT = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? jT(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && BT(e, o, i), i;
}, HT = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, YT = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, KT = (t, e, o) => (HT(t, e, "access private method"), o), Cn, ah;
const XT = "forms-buy-license-modal";
let pi = class extends ge {
  constructor() {
    super(...arguments), YT(this, Cn);
  }
  render() {
    return n`<umb-body-layout
      .headline=${this.localize.term("formsDashboard_buyLicenseTitle")}
    >
      ${co(this.localize.term("formsDashboard_buyLicenseCopy"))}
      <hr />
      ${co(this.localize.term("formsDashboard_buyLicenseFAQs"))}
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          @click=${KT(this, Cn, ah)}
          look="primary"
          color="positive"
          .label=${this.localize.term("formsDashboard_buyLink")}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
Cn = /* @__PURE__ */ new WeakSet();
ah = function() {
  window.open(
    "https://shop.umbraco.com/shop/cart/?p=6201&amp;direct=true&amp;utm_source=core&amp;utm_medium=dashboard&amp;utm_content=topic-link&amp;utm_campaign=formslicensing",
    "_blank"
  );
};
pi.styles = [
  C`
      :host {
        display: block;
        max-width: 80vw;
        width: 800px;
      }
    `
];
pi = GT([
  h(XT)
], pi);
const QT = pi, JT = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsBuyLicenseModalElement() {
    return pi;
  },
  default: QT
}, Symbol.toStringTag, { value: "Module" }));
var ZT = Object.defineProperty, e$ = Object.getOwnPropertyDescriptor, ct = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? e$(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && ZT(e, o, i), i;
}, zc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, On = (t, e, o) => (zc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), Ce = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, t$ = (t, e, o, r) => (zc(t, e, "write to private field"), e.set(t, o), o), ne = (t, e, o) => (zc(t, e, "access private method"), o), yo, Pn, sh, kn, nh, Nc, lh, Dn, ch, qc, uh, gr, xn, _r, Mn, Ro, vr;
const o$ = "forms-configure-license-modal";
let de = class extends ge {
  constructor() {
    super(), Ce(this, Pn), Ce(this, kn), Ce(this, Nc), Ce(this, Dn), Ce(this, qc), Ce(this, gr), Ce(this, _r), Ce(this, Ro), Ce(this, yo, void 0), this._isLoading = !1, this._loginError = !1, this._configuration = {
      domain: window.location.hostname,
      email: "",
      password: "",
      license: ""
    }, this._configuredLicenses = [], this._openLicenses = [], this.consumeContext(Fo, (t) => {
      t$(this, yo, t);
    });
  }
  render() {
    return n`<umb-body-layout
      .headline=${this.localize.term("formsDashboard_configureLicenseTitle")}
    >
      <p>${this.localize.term("formsDashboard_configureLicenseCopy")}</p>

      ${m(this._isLoading, () => n`<uui-loader></uui-loader>`)}
      ${m(
      !this._isLoading && !this._hasLicenses,
      () => {
        var t, e;
        return n`<p>
            <strong>${this.localize.term("general_email")}:</strong><br />
            <uui-input
              label="email"
              .value=${(t = this._configuration) == null ? void 0 : t.email}
              @change=${ne(this, Pn, sh)}
            ></uui-input
            ><br />
            <strong>${this.localize.term("general_password")}:</strong><br />
            <uui-input
              type="password"
              label="password"
              .value=${(e = this._configuration) == null ? void 0 : e.password}
              @change=${ne(this, kn, nh)}
              spellcheck="false"
            ></uui-input
            ><br />
            <uui-button
              look="primary"
              color="positive"
              @click=${ne(this, Dn, ch)}
              .label=${this.localize.term("formsDashboard_findLicenses")}
            ></uui-button>

            ${m(
          this._loginError,
          () => ne(this, Ro, vr).call(this, "formsDashboard_invalidEmail")
        )}
            ${m(
          !!this._hasLicenses,
          () => ne(this, Ro, vr).call(this, "formsDashboard_noLicensesAvailable")
        )}
          </p>
          <hr />
          ${co(
          this.localize.term("formsDashboard_configureLicenseFAQs")
        )}`;
      }
    )}
      ${m(
      this._hasLicenses,
      () => n` <h5>
            ${this.localize.term("formsDashboard_availableLicensesTitle")}
          </h5>
          <p>
            ${this.localize.term("formsDashboard_foundLicenses")}
            <strong
              >${this._openLicenses.length + this._configuredLicenses.length}</strong
            >
            ${this.localize.term("formsDashboard_foundLicensesAccount")}
            <strong>${this._configuration.email}}</strong>
          </p>
          ${m(
        this._openLicenses.length,
        () => n`
              <ul>
                ${this._openLicenses.map(
          (t) => n`<li style="max-width:600px">
                    <uui-icon name="icon-certificate"></uui-icon>
                    ${ne(this, _r, Mn).call(this, t)}
                    <small
                      >${this.localize.term(
            "formsDashboard_noDomainsAssigned"
          )}</small
                    >
                    ${m(
            t === this._selectedLicense,
            () => n`<p>
                          ${this.localize.term("formsDashboard_setDomain")}:
                        </p>
                        <uui-input
                          label="domain"
                          value=${this._configuration.domain}
                        ></uui-input>
                        ${ne(this, gr, xn).call(this, "formsDashboard_configureAndInstall")} `
          )}
                  </li>`
        )}
              </ul>
              <hr />
            `
      )}

          <ul>
            ${this._configuredLicenses.map(
        (t) => n`<li>
                  <uui-icon name="icon-certificate"></uui-icon>
                  ${ne(this, _r, Mn).call(this, t)}
                  <small>
                    ${m(
          t.bindings.includes(this._configuration.domain),
          () => n` <uui-icon
                        name="icon-check"
                        title=${this.localize.term(
            "formsDashboard_licenseValidOnDomain"
          )}
                      ></uui-icon>`,
          () => ne(this, Ro, vr).call(this, "formsDashboard_licenseNotValidOnDomain")
        )}
                    ${this.localize.term("formsDashboard_validDomains")}:
                    ${t.bindings.join()}
                  </small>

                  ${m(
          t === this._selectedLicense,
          () => ne(this, gr, xn).call(this, "formsDashboard_installLicense")
        )}
                </li>`
      )}
          </ul>`
    )}
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
yo = /* @__PURE__ */ new WeakMap();
Pn = /* @__PURE__ */ new WeakSet();
sh = function(t) {
  this._configuration && (this._configuration.email = t.target.value.toString());
};
kn = /* @__PURE__ */ new WeakSet();
nh = function(t) {
  this._configuration && (this._configuration.password = t.target.value.toString());
};
Nc = /* @__PURE__ */ new WeakSet();
lh = async function() {
  var e, o;
  this._isLoading = !0;
  const { error: t } = await lo(xd.postLicensingConfigure({
    requestBody: this._configuration
  }));
  t ? (e = On(this, yo)) == null || e.peek("danger", {
    data: { message: "An error occurred configuring the license." }
  }) : ((o = On(this, yo)) == null || o.peek("positive", {
    data: { message: this.localize.term("formsDashboard_licenseConfiguredNotificationTitle") }
  }), this._submitModal());
};
Dn = /* @__PURE__ */ new WeakSet();
ch = async function() {
  var e;
  this._loginError = !1, this._hasLicenses = void 0, this._isLoading = !0;
  const { data: t } = await y(
    this,
    xd.postLicensingAvailable({
      requestBody: this._configuration
    })
  );
  t && (this._hasLicenses = t.length > 0, this._configuredLicenses = t.filter((o) => o.configured).sort(
    (o, r) => Number(o.bindings.includes(this._configuration.domain)) - Number(r.bindings.includes(this._configuration.domain))
  ), this._openLicenses = t.filter((o) => !o.configured), this._hasLicenses || (e = On(this, yo)) == null || e.peek("danger", {
    data: { message: "No licenses could be retrieved with the provided credentials." }
  })), this._isLoading = !1;
};
qc = /* @__PURE__ */ new WeakSet();
uh = function(t) {
  this._configuration.license = t.id, this._selectedLicense = t;
};
gr = /* @__PURE__ */ new WeakSet();
xn = function(t) {
  return n`
      <uui-button
        look="primary"
        color="positive"
        @click=${ne(this, Nc, lh)}
        .label=${this.localize.term(t)}
      ></uui-button>
    `;
};
_r = /* @__PURE__ */ new WeakSet();
Mn = function(t) {
  return n`<uui-button
      look="primary"
      color="positive"
      @click=${() => ne(this, qc, uh).call(this, t)}
      .label=${t.label}
    ></uui-button>`;
};
Ro = /* @__PURE__ */ new WeakSet();
vr = function(t) {
  return n`<span class="alert"
      ><uui-icon name="icon-alert"></uui-icon> ${this.localize.term(
    t
  )}</span
    >`;
};
de.styles = [
  C`
      :host {
        display: block;
        max-width: 80vw;
        width: 800px;
      }
      .alert {
        display: flex;
        align-items: center;
        background: var(--uui-color-danger);
        border-radius: var(--uui-border-radius);
        color: var(--uui-color-danger-contrast);
        padding: var(--uui-size-3);
        margin-top: var(--uui-size-5);
      }

      uui-button {
        margin-top: var(--uui-size-3);
      }

      .alert uui-icon {
        margin-right: var(--uui-size-3);
      }
    `
];
ct([
  w()
], de.prototype, "_isLoading", 2);
ct([
  w()
], de.prototype, "_hasLicenses", 2);
ct([
  w()
], de.prototype, "_loginError", 2);
ct([
  w()
], de.prototype, "_configuration", 2);
ct([
  w()
], de.prototype, "_configuredLicenses", 2);
ct([
  w()
], de.prototype, "_openLicenses", 2);
ct([
  w()
], de.prototype, "_selectedLicense", 2);
de = ct([
  h(o$)
], de);
const i$ = de, r$ = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get FormsConfigureLicenseModalElement() {
    return de;
  },
  default: i$
}, Symbol.toStringTag, { value: "Module" })), dh = "Umb.Modal.Forms.BuyLicense", a$ = new $(
  dh,
  {
    modal: {
      type: "dialog",
      size: "medium"
    }
  }
), mh = "Umb.Modal.Forms.ConfigureLicense", s$ = new $(
  mh,
  {
    modal: {
      type: "dialog",
      size: "medium"
    }
  }
), n$ = [
  {
    type: "modal",
    alias: dh,
    name: "Forms Buy License Modal",
    js: () => Promise.resolve().then(() => JT)
  },
  {
    type: "modal",
    alias: mh,
    name: "Forms Configure License Modal",
    js: () => Promise.resolve().then(() => r$)
  }
], l$ = [...n$], c$ = [
  {
    type: "dashboard",
    alias: "forms.dashboard",
    name: "Forms Dashboard",
    weight: 100,
    elementName: "forms-dashboard",
    js: () => import("./forms-dashboard.element.js"),
    meta: {
      label: "Forms",
      pathname: "forms"
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Forms"
      }
    ]
  }
], u$ = [...c$, ...l$], d$ = [
  ...fv,
  ...gS,
  ...OS,
  ...sE,
  ...HE,
  ...Yy,
  ...VT,
  ...u$
], m$ = [
  {
    type: "modal",
    alias: Yd,
    name: "Forms Edit Page Modal",
    js: () => Promise.resolve().then(() => P_)
  },
  {
    type: "modal",
    alias: Kd,
    name: "Forms Edit Fieldset Modal",
    js: () => Promise.resolve().then(() => I_)
  },
  {
    type: "modal",
    alias: Xd,
    name: "Forms Edit Field Modal",
    js: () => Promise.resolve().then(() => q_)
  },
  {
    type: "modal",
    alias: Gd,
    name: "Forms Configure Workflow Modal",
    js: () => Promise.resolve().then(() => m_)
  },
  {
    type: "modal",
    alias: Hd,
    name: "Forms Create Form From DataSource Modal",
    js: () => Promise.resolve().then(() => S_)
  },
  {
    type: "modal",
    alias: Qd,
    name: "Forms Edit Submit Message Modal",
    js: () => Promise.resolve().then(() => Y_)
  },
  {
    type: "modal",
    alias: Zd,
    name: "Forms Edit Workflow Modal",
    js: () => import("./form-edit-workflow-modal.element.js")
  },
  {
    type: "modal",
    alias: Vd,
    name: "Forms Choose Field Type Modal",
    js: () => Promise.resolve().then(() => Gg)
  },
  {
    type: "modal",
    alias: jd,
    name: "Forms Choose Workflow Type Modal",
    js: () => Promise.resolve().then(() => o_)
  },
  {
    type: "modal",
    alias: tm,
    name: "Forms Entry Details Modal",
    js: () => import("./form-entry-details-modal.element.js")
  },
  {
    type: "modal",
    alias: om,
    name: "Forms Export Entries Modal",
    js: () => Promise.resolve().then(() => av)
  }
], p$ = [...m$], h$ = [
  {
    type: "propertyEditorUi",
    alias: "Forms.PropertyEditorUi.FormPicker.Single",
    name: "Single Form Picker Property Editor",
    js: () => import("./form-picker-single-property-editor.element.js"),
    meta: {
      label: "Form Picker (Single)",
      propertyEditorSchemaAlias: "UmbracoForms.FormPicker",
      icon: "icon-umb-contour",
      group: "forms"
    }
  },
  {
    type: "propertyEditorUi",
    alias: "Forms.PropertyEditorUi.FormPicker.Multiple",
    name: "Multiple Form Picker Property Editor",
    js: () => import("./form-picker-multiple-property-editor.element.js"),
    meta: {
      label: "Form Picker (Multiple)",
      propertyEditorSchemaAlias: "UmbracoForms.FormPicker",
      icon: "icon-umb-contour",
      group: "forms"
    }
  }
], f$ = {
  type: "propertyEditorSchema",
  name: "Form Picker",
  alias: "UmbracoForms.FormPicker",
  meta: {
    defaultPropertyEditorUiAlias: "Forms.PropertyEditorUi.FormPicker.Single",
    settings: {
      properties: [
        {
          alias: "allowedFolders",
          label: "Allowed Folders",
          description: "Select the folders from which forms that can be chosen in the picker, or leave empty to allow all folders to be used.",
          propertyEditorUiAlias: "Forms.PropertyEditorUi.FolderPicker.Multiple"
        },
        {
          alias: "allowedForms",
          label: "Allowed Forms",
          description: "Select the individual forms that can be chosen in the picker, or leave empty to allow all forms to be used.",
          propertyEditorUiAlias: "Forms.PropertyEditorUi.FormPicker.Multiple"
        }
      ]
    }
  }
}, y$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.FormDetailsPicker",
  name: "Form Details Picker Property Editor",
  js: () => import("./form-details-picker-property-editor.element.js"),
  meta: {
    label: "Form Details Picker",
    propertyEditorSchemaAlias: "UmbracoForms.FormDetailsPicker",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, g$ = {
  type: "propertyEditorSchema",
  name: "Form Details Picker",
  alias: "UmbracoForms.FormDetailsPicker",
  meta: {
    defaultPropertyEditorUiAlias: "Forms.PropertyEditorUi.FormDetailsPicker",
    settings: {
      properties: [
        {
          alias: "allowedFolders",
          label: "Allowed Folders",
          description: "Select the folders from which forms that can be chosen in the picker, or leave empty to allow all folders to be used.",
          propertyEditorUiAlias: "Forms.PropertyEditorUi.FolderPicker.Multiple"
        },
        {
          alias: "allowedForms",
          label: "Allowed Forms",
          description: "Select the individual forms that can be chosen in the picker, or leave empty to allow all forms to be used.",
          propertyEditorUiAlias: "Forms.PropertyEditorUi.FormPicker.Multiple"
        },
        {
          alias: "includeThemePicker",
          label: "Include Theme Picker",
          description: "Select whether to allow editors to select the theme for the form.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
        },
        {
          alias: "includeRedirectPicker",
          label: "Include Redirect Picker",
          description: "Select whether to allow editors to override the page to redirect to after the form is submitted.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
        }
      ]
    }
  }
}, _$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.FolderPicker.Multiple",
  name: "Multiple Folder Picker Property Editor",
  js: () => import("./folder-picker-multiple-property-editor.element.js"),
  meta: {
    label: "Folder Picker (Multiple)",
    propertyEditorSchemaAlias: "UmbracoForms.FolderPicker",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, v$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.ThemePicker",
  name: "Theme Picker Property Editor",
  js: () => import("./theme-picker-property-editor.element.js"),
  meta: {
    label: "Theme Picker",
    propertyEditorSchemaAlias: "UmbracoForms.ThemePicker",
    icon: "icon-brush",
    group: "forms"
  }
}, S$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.DataTypePicker",
  name: "Forms Data Type Picker Property Editor",
  js: () => import("./data-type-picker-property-editor.element.js"),
  meta: {
    label: "Data Type Picker",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, w$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.DocumentMapper",
  name: "Forms Document Mapper Property Editor",
  js: () => import("./document-mapper-property-editor.element.js"),
  meta: {
    label: "Document Mapper",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, b$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.DocumentTypePicker",
  name: "Forms Document Type Picker Property Editor",
  js: () => import("./document-type-picker-property-editor.element.js"),
  meta: {
    label: "Document Type Picker",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, F$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.DocumentTypeFieldPicker",
  name: "Forms Document Type Field Picker Property Editor",
  js: () => import("./document-type-field-picker-property-editor.element.js"),
  meta: {
    label: "Document Type Field Picker",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, E$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.EmailTemplatePicker",
  name: "Forms Email Template Picker Property Editor",
  js: () => import("./email-template-picker-property-editor.element.js"),
  meta: {
    label: "Email Template Picker",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, T$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.FieldMapper",
  name: "Forms Field Mapper Property Editor",
  js: () => import("./field-mapper-property-editor.element.js"),
  meta: {
    label: "Field Mapper",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, $$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.Password",
  name: "Forms Password Property Editor",
  js: () => import("./password-property-editor.element.js"),
  meta: {
    label: "Password",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, C$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.StandardFieldMapper",
  name: "Forms Standard Field Mapper Property Editor",
  js: () => import("./standard-field-mapper-property-editor.element.js"),
  meta: {
    label: "Standard Field Mapper",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, O$ = {
  type: "propertyEditorUi",
  alias: "Forms.PropertyEditorUi.TextWithFieldPicker",
  name: "Forms Text With Field Picker Property Editor",
  js: () => import("./text-with-field-picker-property-editor.element.js"),
  meta: {
    label: "Text with Field Picker",
    icon: "icon-umb-contour",
    group: "forms"
  }
}, P$ = [
  ...h$,
  y$,
  g$,
  f$,
  _$,
  v$,
  S$,
  b$,
  F$,
  w$,
  E$,
  T$,
  $$,
  C$,
  O$
], k$ = {
  type: "globalContext",
  alias: "Forms.GlobalContext",
  name: "Umbraco Forms Global Context",
  api: () => Promise.resolve().then(() => k_)
}, D$ = [k$], x$ = [
  {
    type: "ufmComponent",
    alias: "Forms.Markdown.FormName",
    name: "Form Name UFM Component",
    api: () => import("./form-name.component.js"),
    meta: { marker: "umbFormName:", alias: "umbFormName" }
  }
];
var M$ = Object.defineProperty, A$ = Object.getOwnPropertyDescriptor, Vc = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? A$(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && M$(e, o, i), i;
}, Bc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, qr = (t, e, o) => (Bc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), zt = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, R$ = (t, e, o, r) => (Bc(t, e, "write to private field"), e.set(t, o), o), Io = (t, e, o) => (Bc(t, e, "access private method"), o), go, jc, An, ph, Rn, hh, Gc, fh, Sr, In;
const I$ = "form-grid";
let hi = class extends st(It) {
  constructor() {
    super(...arguments), zt(this, An), zt(this, Rn), zt(this, Gc), zt(this, Sr), zt(this, go, 10), zt(this, jc, 10), this._forms = [];
  }
  async connectedCallback() {
    super.connectedCallback(), this._forms = await K.getForm();
  }
  render() {
    return n`<div class="header">
        <h4>${this.localize.term("formsDashboard_yourForms")}</h4>
        ${Io(this, Sr, In).call(this)}
      </div>
      <uui-ref-list>
        ${this._forms.slice(0, qr(this, go) - 1).map(
      (t) => n`<ref-form
              .model=${t}
              .config=${this.config}
              @edit=${() => Io(this, An, ph).call(this, t.id)}
              @view=${() => Io(this, Rn, hh).call(this, t.id)}
            ></ref-form>`
    )} </uui-ref-list
      >
      <div class="footer">${Io(this, Sr, In).call(this)}</div>`;
  }
};
go = /* @__PURE__ */ new WeakMap();
jc = /* @__PURE__ */ new WeakMap();
An = /* @__PURE__ */ new WeakSet();
ph = function(t) {
  history.pushState(
    {},
    "",
    `/umbraco/section/forms/workspace/form/edit/${t}`
  );
};
Rn = /* @__PURE__ */ new WeakSet();
hh = function(t) {
  history.pushState(
    {},
    "",
    `/umbraco/section/forms/workspace/form/edit/${t}/view/entries`
  );
};
Gc = /* @__PURE__ */ new WeakSet();
fh = function() {
  R$(this, go, qr(this, go) + qr(this, jc)), this.requestUpdate();
};
Sr = /* @__PURE__ */ new WeakSet();
In = function() {
  if (!(this._forms.length <= qr(this, go)))
    return n` <uui-button
      @click=${Io(this, Gc, fh)}
      .label=${this.localize.term("formsDashboard_showMore")}
    ></uui-button>`;
};
hi.styles = [
  C`
      .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }
      .footer {
        margin-top: 10px;
        width: 100%;
        text-align: right;
      }
    `
];
Vc([
  p({ type: Object })
], hi.prototype, "config", 2);
Vc([
  w()
], hi.prototype, "_forms", 2);
hi = Vc([
  h(I$)
], hi);
var U$ = Object.defineProperty, W$ = Object.getOwnPropertyDescriptor, yh = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? W$(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && U$(e, o, i), i;
}, L$ = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, cr = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Vr = (t, e, o) => (L$(t, e, "access private method"), o), Hc, gh, Yc, _h, Un, vh, Wn, Sh;
const z$ = "forms-licensing";
let Br = class extends st(
  It
) {
  constructor() {
    super(...arguments), cr(this, Hc), cr(this, Yc), cr(this, Un), cr(this, Wn);
  }
  render() {
    var t, e;
    return m(
      ((t = this.status) == null ? void 0 : t.isInvalid) || ((e = this.status) == null ? void 0 : e.isTrial),
      () => {
        var o;
        return n`<uui-box>
        ${m((o = this.status) == null ? void 0 : o.isInvalid, () => Vr(this, Un, vh).call(this))}
        ${Vr(this, Wn, Sh).call(this)}
      </uui-box> `;
      }
    );
  }
};
Hc = /* @__PURE__ */ new WeakSet();
gh = async function() {
  await (await this.getContext(D)).open(this, a$).onSubmit().catch(() => {
  });
};
Yc = /* @__PURE__ */ new WeakSet();
_h = async function() {
  await (await this.getContext(D)).open(this, s$).onSubmit().catch(() => {
  });
};
Un = /* @__PURE__ */ new WeakSet();
vh = function() {
  var t;
  return n`<div class="alert alert-error">
      <h4>${this.localize.term("formsDashboard_invalidLicense")}</h4>
      ${this.localize.term("formsDashboard_invalidLicenseValidFor")}
      <pre>${(t = this.status) == null ? void 0 : t.licenseLimitations}</pre>
      ${this.localize.term("formsDashboard_reconfigure")}
    </div>`;
};
Wn = /* @__PURE__ */ new WeakSet();
Sh = function() {
  return n` <div id="noState">
      <div id="noStateHeader">
        <h3>${this.localize.term("formsDashboard_trialTitle")}</h3>
        <p>${this.localize.term("formsDashboard_trialDescription")}</p>
      </div>

      <div id="noStateOptions">
        <uui-button
          look="primary"
          color="positive"
          @click=${Vr(this, Hc, gh)}
          .label=${this.localize.term("formsDashboard_buyLicense")}
        ></uui-button>
        <uui-button
          look="primary"
          color="default"
          @click=${Vr(this, Yc, _h)}
          .label=${this.localize.term("formsDashboard_configureLicense")}
        ></uui-button>
      </div>
    </div>`;
};
Br.styles = [
  C`
      .alert {
        border-left: 4px solid;
        border-radius: var(--uui-border-radius);
        padding: var(--uui-size-3);
      }

      .alert-error {
        border-left-color: var(--uui-color-danger);
      }
    `
];
yh([
  p({ type: Object })
], Br.prototype, "status", 2);
Br = yh([
  h(z$)
], Br);
var N$ = Object.defineProperty, q$ = (t, e, o, r) => {
  for (var i = void 0, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = s(e, o, i) || i);
  return i && N$(e, o, i), i;
}, Ft, ca, wh;
const Pu = class Pu extends st(
  It
) {
  constructor() {
    super();
    f(this, ca);
    f(this, Ft, void 0);
    this.consumeContext(Ae, (o) => {
      o && (_(this, Ft, o), F(this, ca, wh).call(this));
    });
  }
  onObserveForm() {
  }
  getAllFieldsForForm() {
    var o;
    return (o = d(this, Ft)) == null ? void 0 : o.getAllFields();
  }
  setPropertyValue(o, r) {
    var i;
    (i = d(this, Ft)) == null || i.setFormProperty(o, r), this.dispatchEvent(new CustomEvent("valueChange"));
  }
};
Ft = new WeakMap(), ca = new WeakSet(), wh = function() {
  this.observe(d(this, Ft).data, (o) => {
    o && (this.form = o, this.onObserveForm());
  });
}, Pu.styles = [
  C`
      .flex {
        display: flex;
        flex-direction: column;
      }

      .flex + .flex {
        margin-top: var(--uui-size-3);
      }

      .flex.gap {
        margin-top: var(--uui-size-5);
      }

      .flex + uui-toggle {
        display: block;
        margin-top: var(--uui-size-3);
      }
    `
];
let jr = Pu;
q$([
  w()
], jr.prototype, "form");
var V$ = Object.defineProperty, B$ = Object.getOwnPropertyDescriptor, bh = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? B$(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && V$(e, o, i), i;
}, Kc = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, es = (t, e, o) => (Kc(t, e, "read from private field"), o ? o.call(t) : e.get(t)), te = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, j$ = (t, e, o, r) => (Kc(t, e, "write to private field"), e.set(t, o), o), k = (t, e, o) => (Kc(t, e, "access private method"), o), Yt, Vo, Gr, ir, Ma, rr, Aa, Ra, Xc, Ia, Qc, Ua, Jc, Ln, Fh, zn, Eh, Nn, Th, qn, $h, Hr, Zc, Vn, Ch;
const G$ = "forms-advanced-validation-rules";
let Yr = class extends jr {
  constructor() {
    super(), te(this, Vo), te(this, ir), te(this, rr), te(this, Ra), te(this, Ia), te(this, Ua), te(this, Ln), te(this, zn), te(this, Nn), te(this, qn), te(this, Hr), te(this, Vn), te(this, Yt, void 0), this._editIndex = -1, this.consumeContext(Fo, (t) => {
      j$(this, Yt, t);
    });
  }
  render() {
    return n`<umb-property-layout
      alias="validationRules"
      .label=${this.localize.term("formAdvanced_validationRules")}
      .description=${this.localize.term("formAdvanced_validationRulesDescription")}
    >
      <div slot="editor">
        <div class="flex">
          <uui-table>
            <uui-table-head>
              <uui-table-head-cell>
                Rule
                <small>${co(this.localize.term("formAdvanced_validationRulesRuleDescription"))}</small>
              </uui-table-head-cell>
              <uui-table-head-cell>
                Message
                <small>${this.localize.term("formAdvanced_validationRulesErrorMessageDescription")}</small>
              </uui-table-head-cell>
              <uui-table-head-cell>
                Field
                <small>${this.localize.term("formAdvanced_validationRulesFieldDescription")}</small>
              </uui-table-head-cell>
              <uui-table-head-cell>
              </uui-table-head-cell>
            </uui-table-head>
              ${this.form.validationRules.map(
      (t, e) => n` <uui-table-row>
                  <uui-table-cell><code>${k(this, Hr, Zc).call(this, t.rule)}</code></uui-table-cell>
                  <uui-table-cell>${t.errorMessage}</uui-table-cell>
                  <uui-table-cell>${k(this, Vn, Ch).call(this, t.fieldId)}</uui-table-cell>
                  <uui-table-head-cell>
                    <uui-action-bar>
                      <uui-button
                        label="edit"
                        look="secondary"
                        color="default"
                        @click=${() => k(this, Ln, Fh).call(this, e)}
                      >
                        <uui-icon name="edit"></uui-icon>
                      </uui-button>
                      <uui-button
                        label=${this.localize.term("general_delete")}
                        look="secondary"
                        color="default"
                        @click=${() => k(this, qn, $h).call(this, e)}
                      >
                        <uui-icon name="delete"></uui-icon>
                      </uui-button>
                    </uui-action-bar>
                  </uui-table-head-cell>
                </uui-table-row>`
    )}

            <uui-table-row>
              <uui-table-cell>
                <uui-textarea id="rule" rows="8" class="json" placeholder="${this.localize.term("formAdvanced_validationRuleDefinition")}"></uui-textarea>
              </uui-table-cell>
              <uui-table-cell>
                <uui-textarea id="errorMessage" rows="8" placeholder="${this.localize.term("formAdvanced_validationRuleErrorMessage")}"></uui-textarea>
              </uui-table-cell>
              <uui-table-cell>
                <uui-select
                  id="fieldId"
                  .options=${this.getAllFieldsForForm().map((t) => ({
      name: this.localize.term(t.caption),
      value: t.id,
      selected: this._editIndex > -1 && t.id === this.form.validationRules[this._editIndex].fieldId
    }))}
                >
                </uui-select>
              </uui-table-cell>
              <uui-table-cell>
                <uui-action-bar>
                  <uui-button
                    label="add"
                    look="secondary"
                    color="default"
                    @click=${k(this, zn, Eh)}
                  >
                    <uui-icon
                      .name=${k(this, Vo, Gr).call(this) ? "icon-save" : "add"}
                    ></uui-icon>
                  </uui-button>
                  ${m(
      k(this, Vo, Gr).call(this),
      () => n`<uui-button
                      label="add"
                      look="secondary"
                      color="default"
                      @click=${k(this, Nn, Th)}
                      ><uui-icon name="wrong"></uui-icon
                    ></uui-button>`
    )}
                </uui-action-bar>
              </uui-table-cell>
            </uui-table-row>
          </uui-table>
        </div>
      </div>
    </umb-property-layout>`;
  }
};
Yt = /* @__PURE__ */ new WeakMap();
Vo = /* @__PURE__ */ new WeakSet();
Gr = function() {
  return this._editIndex > -1;
};
ir = /* @__PURE__ */ new WeakSet();
Ma = function() {
  return k(this, Ia, Qc).call(this, "rule");
};
rr = /* @__PURE__ */ new WeakSet();
Aa = function() {
  return k(this, Ia, Qc).call(this, "errorMessage");
};
Ra = /* @__PURE__ */ new WeakSet();
Xc = function() {
  var t;
  return (t = this.shadowRoot) == null ? void 0 : t.getElementById("fieldId");
};
Ia = /* @__PURE__ */ new WeakSet();
Qc = function(t) {
  var e;
  return (e = this.shadowRoot) == null ? void 0 : e.getElementById(t);
};
Ua = /* @__PURE__ */ new WeakSet();
Jc = function() {
  k(this, ir, Ma).call(this).value = "", k(this, rr, Aa).call(this).value = "", k(this, Ra, Xc).call(this).value = "", this._editIndex = -1;
};
Ln = /* @__PURE__ */ new WeakSet();
Fh = function(t) {
  this._editIndex = t;
  const e = this.form.validationRules[this._editIndex];
  k(this, ir, Ma).call(this).value = k(this, Hr, Zc).call(this, e.rule), k(this, rr, Aa).call(this).value = e.errorMessage;
};
zn = /* @__PURE__ */ new WeakSet();
Eh = function() {
  var i, a, s;
  let t = k(this, ir, Ma).call(this).value.toString();
  const e = k(this, rr, Aa).call(this).value.toString(), o = k(this, Ra, Xc).call(this).value.toString();
  try {
    t = JSON.stringify(JSON.parse(t));
  } catch {
    const S = {
      data: {
        headline: this.localize.term("formAdvanced_validationRulesErrorTitle"),
        message: this.localize.term("formAdvanced_validationRulesRuleErrorDescription")
      }
    };
    (i = es(this, Yt)) == null || i.peek("danger", S);
    return;
  }
  if (e.trim().length === 0) {
    const u = {
      data: {
        headline: this.localize.term("formAdvanced_validationRulesErrorTitle"),
        message: this.localize.term("formAdvanced_validationRulesMessageErrorDescription")
      }
    };
    (a = es(this, Yt)) == null || a.peek("danger", u);
    return;
  }
  if (o.length === 0) {
    const u = {
      data: {
        headline: this.localize.term("formAdvanced_validationRulesErrorTitle"),
        message: this.localize.term("formAdvanced_validationRulesFieldErrorDescription")
      }
    };
    (s = es(this, Yt)) == null || s.peek("danger", u);
    return;
  }
  const r = structuredClone(this.form.validationRules);
  k(this, Vo, Gr).call(this) ? (r[this._editIndex].rule = t, r[this._editIndex].errorMessage = e, r[this._editIndex].fieldId = o) : r.push({
    rule: t,
    errorMessage: e,
    fieldId: o
  }), this.setPropertyValue("validationRules", r), k(this, Ua, Jc).call(this);
};
Nn = /* @__PURE__ */ new WeakSet();
Th = function() {
  k(this, Ua, Jc).call(this);
};
qn = /* @__PURE__ */ new WeakSet();
$h = async function(t) {
  await Yl(this, {
    headline: this.localize.term("formAdvanced_deleteValidationRuleHeadline"),
    content: this.localize.term("formAdvanced_deleteValidationRuleMessage"),
    confirmLabel: this.localize.term("general_yes"),
    color: "danger"
  });
  const e = structuredClone(this.form.validationRules);
  e.splice(t, 1), this.setPropertyValue("validationRules", e);
};
Hr = /* @__PURE__ */ new WeakSet();
Zc = function(t) {
  var e = JSON.parse(t);
  return JSON.stringify(e, null, 2);
};
Vn = /* @__PURE__ */ new WeakSet();
Ch = function(t) {
  const e = this.getAllFieldsForForm().filter((r) => r.id === t);
  if (e.length === 0)
    return "";
  const o = e[0];
  return o.caption + " (" + o.alias + ")";
};
Yr.styles = [
  C`
      uui-table-head-cell, uui-table-cell {
        vertical-align: top;
      }
      uui-table-head-cell small {
        display: block;
        font-weight: normal;
      }
      uui-table-cell code {
        background-color: var(--uui-color-surface-alt);
        border: solid 1px var(--uui-color-border-standalone);
        display: block;
        white-space: pre;
        font-family: monospace;
        font-size: 13px;
        padding: 4px;
        white-space: pre;
      }
      uui-textarea {
        width: 100%;
      }
      uui-textarea.json {
        font-family: monospace;
      }
    `
];
bh([
  w()
], Yr.prototype, "_editIndex", 2);
Yr = bh([
  h(G$)
], Yr);
var W, Ui, no, wr, Wi, Bn, Li, jn, zi, Gn, Et, Uo, Ni, Hn;
class UP {
  constructor(e, o) {
    // TODO => localize
    f(this, no);
    f(this, Wi);
    f(this, Li);
    f(this, zi);
    f(this, Et);
    f(this, Ni);
    f(this, W, void 0);
    f(this, Ui, void 0);
    if (!o)
      throw new Error("workspaceContext is missing");
    if (!e)
      throw new Error("host is missing");
    _(this, Ui, e), _(this, W, o);
  }
  addFormPage(e = !1) {
    const o = d(this, W).getData();
    if (!o)
      return;
    const r = ot.getPageScaffold();
    r.form = o.unique;
    const i = [...d(this, W).getData().pages];
    e ? i.unshift(r) : i.push(r), d(this, W).setFormProperty("pages", i);
  }
  async deleteFormPage(e) {
    if (!e)
      throw new Error("page is missing");
    F(this, no, wr).call(this, "page", () => {
      const o = d(this, W).getData().pages;
      d(this, W).setFormProperty(
        "pages",
        Mu(o, (r) => r.id !== e.id)
      );
    });
  }
  addFormGroup(e) {
    if (!e)
      throw new Error("page is missing");
    const o = ot.getFieldsetScaffold();
    o.sortOrder = e.fieldSets.length, o.page = e.id, F(this, Et, Uo).call(this, e, o);
  }
  deleteFormGroup(e) {
    if (!e)
      throw new Error("fieldset is missing");
    F(this, no, wr).call(this, "group", () => {
      const o = F(this, Ni, Hn).call(this, e.page), r = [...o.fieldSets];
      r.splice(r.indexOf(e), 1), d(this, W).setFormProperty(
        "pages",
        qt(
          d(this, W).getData().pages,
          {
            ...o,
            fieldSets: r
          },
          (i) => i.id === o.id
        )
      );
    });
  }
  copyFormGroup(e) {
    if (!e)
      throw new Error("fieldset is missing");
    const o = F(this, Ni, Hn).call(this, e.page), r = structuredClone(e);
    r.sortOrder = o.fieldSets.length, r.page = o.id, r.id = q.new(), r.containers.forEach(
      (i) => i.fields.forEach((a) => a.id = q.new())
    ), F(this, Et, Uo).call(this, o, r);
  }
  deleteQuestion(e, o, r) {
    if (!o)
      throw new Error("fieldsetId is missing");
    if (!r)
      throw new Error("pageId is missing");
    F(this, no, wr).call(this, "question", () => {
      const { page: i, fieldset: a, container: s } = F(this, zi, Gn).call(this, e, o, r);
      F(this, Et, Uo).call(this, i, {
        ...a,
        containers: qt(
          a.containers,
          {
            ...s,
            fields: Mu(
              s.fields,
              (u) => u.id !== e.id
            )
          },
          (u) => u === s
        )
      });
    });
  }
  copyQuestion(e, o, r) {
    if (!o)
      throw new Error("fieldsetId is missing");
    if (!r)
      throw new Error("pageId is missing");
    const i = structuredClone(e);
    i.id = q.new(), i.alias = F(this, Wi, Bn).call(this, e.alias, d(this, W).getAllFieldAliases());
    const { fieldset: a, container: s } = F(this, zi, Gn).call(this, e, o, r);
    this.insertQuestion(
      i,
      a,
      s,
      s.fields.indexOf(e) + 1
    );
  }
  /**
   * Handles inserting a copied field into the correct fieldset
   */
  insertQuestion(e, o, r, i) {
    if (!o)
      throw new Error("fieldset is missing");
    if (!r)
      throw new Error("container is missing");
    const a = o.containers.findIndex((u) => u === r), s = Au(r.fields, e, (u) => u.id);
    i !== void 0 && s.splice(i, 0, s.pop()), F(this, Li, jn).call(this, o, r, a, s);
  }
  /** returns an array of objects sorted against the sortedIds array */
  reorderArray(e, o) {
    const r = [...e];
    return r.sort((i, a) => o.indexOf(i.id) - o.indexOf(a.id)), r;
  }
  reorderGroups(e, o) {
    d(this, W).setFormProperty(
      "pages",
      qt(
        d(this, W).getData().pages,
        Yf(e, {
          fieldSets: this.reorderArray(
            e.fieldSets,
            o
          )
        }),
        (r) => r.id === e.id
      )
    );
  }
  reorderQuestions(e, o, r) {
    const i = this.reorderArray(o.fields, r), a = e.containers.findIndex((s) => s === o);
    F(this, Li, jn).call(this, e, o, a, i);
  }
}
W = new WeakMap(), Ui = new WeakMap(), no = new WeakSet(), wr = async function(e, o) {
  await Yl(d(this, Ui), {
    headline: `Delete ${e}`,
    content: `Are you sure you want to delete this ${e}?`,
    confirmLabel: "Delete",
    color: "danger"
  }), o();
}, Wi = new WeakSet(), Bn = function(e, o) {
  let r = "";
  const i = e.match(/\d+$/);
  if (i) {
    const s = e.substring(0, e.length - i[0].length), S = parseInt(i[0], 10) + 1;
    r = s + S;
  } else
    r = e + "2";
  let a = !1;
  for (let s = 0; s < o.length; s++)
    if (o[s] === r) {
      a = !0;
      break;
    }
  return a ? F(this, Wi, Bn).call(this, r, o) : r;
}, Li = new WeakSet(), jn = function(e, o, r, i) {
  const a = {
    ...o,
    fields: i
  }, s = {
    ...e,
    containers: qt(
      e.containers,
      a,
      (g) => e.containers.indexOf(g) === r
    )
  }, S = d(this, W).getData().pages.find((g) => g.id === s.page);
  F(this, Et, Uo).call(this, S, s);
}, zi = new WeakSet(), Gn = function(e, o, r) {
  function i(g, b) {
    return g.length === 1 ? g[0] : g.find(b);
  }
  const a = d(this, W).getData(), s = i(a.pages, (g) => g.id === r), u = i(s.fieldSets, (g) => g.id === o), S = i(
    u.containers,
    (g) => g.fields.some((b) => b.id === e.id)
  );
  return { page: s, fieldset: u, container: S };
}, Et = new WeakSet(), Uo = function(e, o) {
  const r = {
    ...e,
    fieldSets: Au(e.fieldSets, o, (i) => i.id)
  };
  d(this, W).setFormProperty(
    "pages",
    qt(
      d(this, W).getData().pages,
      r,
      (i) => i.id === r.id
    )
  );
}, Ni = new WeakSet(), Hn = function(e) {
  const o = d(this, W).getData().pages.find((r) => r.id === e);
  if (!o)
    throw new Error("page is missing");
  return o;
};
const H$ = new M("FormStructureManager");
var Y$ = Object.defineProperty, K$ = Object.getOwnPropertyDescriptor, Wa = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? K$(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Y$(e, o, i), i;
}, Tt, Hl, X$;
class Ee extends st(
  It
) {
  constructor() {
    super();
    f(this, Hl);
    f(this, Tt, void 0);
    this.index = 0, this.allFields = [], this.allFieldTypes = [], _(this, Tt, !1), this.consumeContext(Ae, (o) => {
      this.workspaceContext = o;
    }), this.consumeContext(D, (o) => {
      this.modalContext = o;
    }), this.consumeContext(H$, (o) => {
      this.structureManager = o;
    });
  }
  set sortModeActive(o) {
    var r, i;
    _(this, Tt, o), d(this, Tt) ? (r = this.sortModeEnabled) == null || r.call(this) : (i = this.sortModeDisabled) == null || i.call(this);
  }
  get sortModeActive() {
    return d(this, Tt);
  }
  async editNewOrExistingField(o, r, i) {
    if (!this.modalContext)
      return;
    const a = await this.workspaceContext.loadFieldType(
      o.fieldTypeId
    );
    if (!a)
      throw new Error(
        "Field type with id " + o.fieldTypeId + " could not be found."
      );
    const s = this.workspaceContext.getAllPages();
    let u = [];
    if (a.supportsRegex) {
      const Te = await this.workspaceContext.loadValidationPatterns();
      Te && (u = Te);
    }
    let S = [];
    if (a.supportsPrevalues) {
      const Te = await this.workspaceContext.loadPrevalueSources();
      Te && (S = Te);
    }
    const g = o.caption, b = o.alias, I = o.tooltip, B = o.containsSensitiveData, Y = o.allowedUploadTypes, E = o.allowMultipleFileUploads, J = o.preValues, _e = o.prevalueSourceId, U = o.mandatory, Lf = o.requiredErrorMessage, zf = o.regex, Nf = o.invalidErrorMessage, qf = o.condition ? structuredClone(o.condition) : ot.getConditionScaffold(), Du = o.settings;
    if (r)
      for (let Te = 0; Te < a.settings.length; Te++) {
        const Ha = a.settings[Te];
        Ha.defaultValue && (Du[Ha.alias] = Ha.defaultValue);
      }
    const Vf = this.workspaceContext.getContainerIndexPathForField(o.id);
    this.modalContext.open(this, Mg, {
      data: {
        fields: this.allFields,
        pages: s,
        validationPatterns: u,
        prevalueSources: S,
        isNew: r
      },
      value: {
        caption: g,
        alias: b,
        tooltip: I,
        fieldType: a,
        containsSensitiveData: B,
        allowedUploadTypes: Y,
        allowMultipleFileUploads: E,
        prevalues: J,
        prevalueSourceId: _e,
        mandatory: U,
        requiredErrorMessage: Lf,
        regex: zf,
        invalidErrorMessage: Nf,
        condition: qf,
        settings: Du,
        containerIndexPath: Vf
      }
    }).onSubmit().then(i).catch(() => {
    });
  }
}
Tt = new WeakMap(), Hl = new WeakSet(), X$ = function(o, r, i, a, s, u) {
  this.workspaceContext.setFieldProperty(
    o,
    r,
    i,
    a,
    s,
    u
  );
};
Wa([
  p({ type: Number })
], Ee.prototype, "index", 2);
Wa([
  p({ type: Array })
], Ee.prototype, "allFields", 2);
Wa([
  p({ type: Array })
], Ee.prototype, "allFieldTypes", 2);
Wa([
  p({ type: Boolean, reflect: !0, attribute: "sort-mode-active" })
], Ee.prototype, "sortModeActive", 1);
var Q$ = Object.defineProperty, J$ = Object.getOwnPropertyDescriptor, Oh = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? J$(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && Q$(e, o, i), i;
}, eu = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, br = (t, e, o) => (eu(t, e, "read from private field"), o ? o.call(t) : e.get(t)), Oe = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Yn = (t, e, o, r) => (eu(t, e, "write to private field"), e.set(t, o), o), mt = (t, e, o) => (eu(t, e, "access private method"), o), La, Ct, Kn, Ph, Xn, kh, Qn, Dh, Jn, xh, Zn, Mh, el, Ah, tu, Rh;
const Z$ = "forms-form-page";
let Kr = class extends Ee {
  constructor() {
    super(...arguments), Oe(this, Kn), Oe(this, Xn), Oe(this, Qn), Oe(this, Jn), Oe(this, Zn), Oe(this, el), Oe(this, tu), Oe(this, La, void 0), Oe(this, Ct, []);
  }
  sortModeEnabled() {
    mt(this, Kn, Ph).call(this), mt(this, Xn, kh).call(this);
  }
  render() {
    var t, e, o;
    return n`<div id="pageHeader">
        <div id="pageNumber">${this.index + 1}</div>
          <uui-input
            .value=${(t = this.page) == null ? void 0 : t.caption}
            @change=${mt(this, el, Ah)}
            label="caption"
            .placeholder=${this.localize.term("formEdit_pageTitlePlaceholder")}
          ></uui-input>
          ${m(
      !this.sortModeActive,
      () => n`<div id="pageActions">
              <uui-action-bar>
                <uui-button
                  label="edit"
                  look="secondary"
                  color="default"
                  @click=${mt(this, Qn, Dh)}
                >
                  <uui-icon name="settings"></uui-icon>
                </uui-button>
                <uui-button
                  label=${this.localize.term("general_delete")}
                  look="secondary"
                  color="default"
                  @click=${mt(this, Jn, xh)}
                >
                  <uui-icon name="delete"></uui-icon>
                </uui-button>
              </uui-action-bar>
            </div>`
    )}
      </div>

      ${m(
      !this.sortModeActive,
      () => {
        var r, i;
        return n`
          <forms-form-condition-summary
            formElement="pageButton"
            .condition=${(r = this.page) == null ? void 0 : r.buttonCondition}
            .operatorTypes=${((i = this.workspaceContext) == null ? void 0 : i.getConditionOperators) ?? []}
            .allFields=${this.allFields}
          >
          </forms-form-condition-summary>`;
      }
    )}

      <div id="pageFieldsets" class="page-${(e = this.page) == null ? void 0 : e.id}">
        ${(o = this.page) == null ? void 0 : o.fieldSets.map(
      (r, i) => {
        var a;
        return n`<forms-form-fieldset
              class="fieldset-${(a = this.page) == null ? void 0 : a.id}"
              sort-unique="${r.id}"
              .fieldset=${r}
              .index=${i}
              .pageIndex=${this.index}
              .allFields=${this.allFields}
              .allFieldTypes=${this.allFieldTypes}
              .sortModeActive=${this.sortModeActive}
            ></forms-form-fieldset>`;
      }
    )}
      </div>
      <div id="pageFooter">
        ${m(
      !this.sortModeActive,
      () => n`
            <uui-button
              @click=${mt(this, Zn, Mh)}
              look="outline"
              label=${this.localize.term("formEdit_addGroup")}
            ></uui-button>`
    )}
      </div>`;
  }
};
La = /* @__PURE__ */ new WeakMap();
Ct = /* @__PURE__ */ new WeakMap();
Kn = /* @__PURE__ */ new WeakSet();
Ph = function() {
  var t, e;
  Yn(this, La, new ji(this, {
    ...new Yi(
      "Forms.SorterIdentifier.Fieldset",
      `forms-form-fieldset.fieldset-${(t = this.page) == null ? void 0 : t.id}`,
      `.page-${(e = this.page) == null ? void 0 : e.id}`
    ).config,
    onChange: ({ model: o }) => {
      Yn(this, Ct, o);
    },
    onEnd: () => {
      var o;
      (o = this.structureManager) == null || o.reorderGroups(this.page, br(this, Ct));
    }
  }));
};
Xn = /* @__PURE__ */ new WeakSet();
kh = function() {
  var e;
  Yn(this, Ct, []);
  const t = this.page.fieldSets;
  for (let o = 0; o < t.length; o++) {
    const r = t[o];
    br(this, Ct).push(r.id);
  }
  (e = br(this, La)) == null || e.setModel(br(this, Ct));
};
Qn = /* @__PURE__ */ new WeakSet();
Dh = async function() {
  var r;
  if (!this.modalContext)
    return;
  const t = (r = this.page) != null && r.buttonCondition ? structuredClone(this.page.buttonCondition) : ot.getConditionScaffold(), o = await this.modalContext.open(this, Dg, {
    data: {
      fields: this.allFields
    },
    value: {
      buttonCondition: t
    }
  }).onSubmit().catch(() => {
  });
  o && mt(this, tu, Rh).call(this, "buttonCondition", o.buttonCondition);
};
Jn = /* @__PURE__ */ new WeakSet();
xh = function() {
  var t;
  (t = this.structureManager) == null || t.deleteFormPage(this.page);
};
Zn = /* @__PURE__ */ new WeakSet();
Mh = function() {
  var t;
  (t = this.structureManager) == null || t.addFormGroup(this.page);
};
el = /* @__PURE__ */ new WeakSet();
Ah = function(t) {
  if (this.page) {
    const e = t.target.value.toString();
    this.workspaceContext.setPageProperty(this.index, "caption", e);
  }
};
tu = /* @__PURE__ */ new WeakSet();
Rh = function(t, e) {
  this.workspaceContext.setPageProperty(this.index, t, e);
};
Kr.styles = [
  C`
      :host {
        display: block;
        background: #e9e9eb;
        padding: 15px;
        margin-bottom: 30px;
        border-radius: 3px;
      }

      #pageHeader {
        margin-bottom: 15px;
        display: flex;
        flex-wrap: wrap;
        flex-direction: row;
        align-items: center;
        font-size: 14px;
        font-weight: bold;
      }
      #pageNumber {
        width: 23px;
        height: 23px;
        line-height: 23px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        background: #ffffff;
        border: 1px solid #bbbabf;
        margin-right: 5px;
      }
      #pageActions {
        color: #817f85;
        display: flex;
        flex-direction: row;
        align-items: flex-start;
        margin-left: auto;
        padding-left: 10px;
      }
      #pageFooter {
        display: flex;
        align-items: center;
        justify-content: center;
        margin-top: 15px;
      }

      forms-form-fieldset + forms-form-fieldset {
        display: block;
        margin-top: var(--uui-size-5);
      }
    `
];
Oh([
  p({ type: Object })
], Kr.prototype, "page", 2);
Kr = Oh([
  h(Z$)
], Kr);
var eC = Object.defineProperty, tC = Object.getOwnPropertyDescriptor, ar = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? tC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && eC(e, o, i), i;
}, oC = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, ed = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, td = (t, e, o) => (oC(t, e, "access private method"), o), tl, Ih, ol, Uh;
const iC = "forms-form-condition-summary";
let At = class extends st(It) {
  constructor() {
    super(...arguments), ed(this, tl), ed(this, ol), this.allFields = [], this.operatorTypes = [], this.formElement = "field";
  }
  render() {
    var t;
    return n`
      ${m(
      (t = this.condition) == null ? void 0 : t.enabled,
      () => {
        var e, o, r;
        return n`<div class="condition-summary">
          ${co(`<span>${this.localize.term("formConditions_" + this.formElement + "ConditionStatus", (e = this.condition) == null ? void 0 : e.actionType, (o = this.condition) == null ? void 0 : o.logicType.toLowerCase())}</span`)}
          ${(r = this.condition) == null ? void 0 : r.rules.map(
          (i, a) => n`<span>
              <strong>${td(this, tl, Ih).call(this, i.field)}</strong>
              ${td(this, ol, Uh).call(this, i.operator)}</span>
              <strong>${i.value.length > 0 ? n`${i.value}` : this.localize.term("formConditions_empty")}</strong>${m(a < this.condition.rules.length - 1, () => n`<span>, </span>`)}
            </span>`
        )}
          </div>`;
      }
    )}`;
  }
};
tl = /* @__PURE__ */ new WeakSet();
Ih = function(t) {
  var e;
  return (e = this.allFields.find((o) => o.id === t)) == null ? void 0 : e.caption;
};
ol = /* @__PURE__ */ new WeakSet();
Uh = function(t) {
  var e;
  return (e = this.operatorTypes.find((o) => o.value === t)) == null ? void 0 : e.name;
};
At.styles = [
  C`
      .condition-summary {
        font-size: 12px;
        margin-top: 8px;
        padding-left: 6px;
        padding-right: 6px;
        text-overflow: ellipsis;
        white-space: nowrap;
        overflow: hidden;
        background: #f6f3fd;
        border: 1px solid #413659;
        border-radius: 3px;
        display: inline-block;
        max-width: 800px;

        span {
          font-style: italic;
        }
      }
    `
];
ar([
  p({ type: Object })
], At.prototype, "condition", 2);
ar([
  p({ type: Array })
], At.prototype, "allFields", 2);
ar([
  p({ type: Array })
], At.prototype, "operatorTypes", 2);
ar([
  p()
], At.prototype, "formElement", 2);
At = ar([
  h(iC)
], At);
var rC = Object.defineProperty, aC = Object.getOwnPropertyDescriptor, Ue = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? aC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && rC(e, o, i), i;
}, sC = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, ae = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, T = (t, e, o) => (sC(t, e, "access private method"), o), il, Wh, za, ou, rl, Lh, al, zh, sl, Nh, nl, qh, ll, Vh, cl, Bh, ul, jh, A, L, dl, Gh, iu, Hh;
const nC = "forms-form-field";
let ye = class extends Ee {
  constructor() {
    super(...arguments), ae(this, il), ae(this, za), ae(this, rl), ae(this, al), ae(this, sl), ae(this, nl), ae(this, ll), ae(this, cl), ae(this, ul), ae(this, A), ae(this, dl), ae(this, iu), this.pageIndex = 0, this.fieldsetIndex = 0, this.containerIndex = 0, this._aliasLocked = !0, this._fieldPrevalues = [];
  }
  async connectedCallback() {
    super.connectedCallback(), await T(this, il, Wh).call(this);
  }
  renderSortableField() {
    return n`<div class="sortable">
      <uui-icon name="icon-navigation"></uui-icon>
      ${this.field.caption}
      <span style="color: var(--uui-color-disabled-contrast)"
        >(${this.field.alias})</span
      >
    </div>`;
  }
  render() {
    return this.sortModeActive ? this.renderSortableField() : n`<div id="fieldHeader">
        <!-- TODO => lifted from doc type header, confirm changes are ok -->
        <uui-input
          id="caption"
          name="caption"
          .value=${this.field.caption}
          @change="${T(this, nl, qh)}"
          label="caption"
        >
          <!-- TODO: should use UUI-LOCK-INPUT, but that does not fire an event when its locked/unlocked -->
          <uui-input
            name="alias"
            slot="append"
            label="alias"
            @change=${T(this, cl, Bh)}
            .value=${this.field.alias}
            placeholder="Enter alias..."
            ?disabled=${this._aliasLocked}
          >
            <!-- TODO: validation for bad characters -->
            <div
              @click=${T(this, ll, Vh)}
              @keydown=${() => ""}
              id="alias-lock"
              slot="prepend"
            >
              <uui-icon
                name=${this._aliasLocked ? "icon-lock" : "icon-unlocked"}
              ></uui-icon>
            </div>
          </uui-input>
        </uui-input>

        <uui-action-bar>
          <uui-button
            label="edit"
            look="secondary"
            color="default"
            @click=${T(this, rl, Lh)}
          >
            <uui-icon name="settings"></uui-icon>
          </uui-button>
          <uui-button
            label="copy"
            look="secondary"
            color="default"
            @click=${T(this, sl, Nh)}
          >
            <uui-icon name="copy"></uui-icon>
          </uui-button>
          <uui-button
            label=${this.localize.term("general_delete")}
            look="secondary"
            color="default"
            @click=${T(this, al, zh)}
          >
            <uui-icon name="delete"></uui-icon>
          </uui-button>
        </uui-action-bar>
      </div>

      <div id="fieldContent">
        <div id="help">
          <uui-textarea
            name="tooltip"
            .placeholder=${this.localize.term("formEdit_helpText")}
            .value=${this.field.tooltip}
            @change="${T(this, ul, jh)}"
            label="tooltip"
          ></uui-textarea>
        </div>
        ${T(this, dl, Gh).call(this, this.field)}
      </div>

      ${m(
      !this.sortModeActive,
      () => {
        var t;
        return n` <forms-form-condition-summary
          formElement="field"
          .condition=${this.field.condition ?? void 0}
          .operatorTypes=${((t = this.workspaceContext) == null ? void 0 : t.getConditionOperators) ?? []}
          .allFields=${this.allFields}
        >
        </forms-form-condition-summary>`;
      }
    )} `;
  }
};
il = /* @__PURE__ */ new WeakSet();
Wh = async function() {
  await T(this, za, ou).call(this, this.field.prevalueSourceId, this.field.preValues);
};
za = /* @__PURE__ */ new WeakSet();
ou = async function(t, e) {
  if (this.workspaceContext)
    if (t !== Dt) {
      const o = new Zp(this), { data: r } = await o.requestPrevalues(
        this.field.prevalueSourceId,
        this.workspaceContext.getUnique(),
        this.field.id
      );
      r && (this._fieldPrevalues = r.map((i) => ({ value: i.value, caption: i.caption })));
    } else
      this._fieldPrevalues = e;
};
rl = /* @__PURE__ */ new WeakSet();
Lh = async function() {
  const t = async (e) => {
    T(this, A, L).call(this, "caption", e.caption), T(this, A, L).call(this, "alias", e.alias), T(this, A, L).call(this, "tooltip", e.tooltip), T(this, A, L).call(this, "fieldTypeId", e.fieldType.id), T(this, A, L).call(this, "containsSensitiveData", e.containsSensitiveData), T(this, A, L).call(this, "allowedUploadTypes", e.allowedUploadTypes), T(this, A, L).call(this, "allowMultipleFileUploads", e.allowMultipleFileUploads), T(this, A, L).call(this, "preValues", e.prevalues), T(this, A, L).call(this, "prevalueSourceId", e.prevalueSourceId), T(this, A, L).call(this, "mandatory", e.mandatory), T(this, A, L).call(this, "requiredErrorMessage", e.requiredErrorMessage), T(this, A, L).call(this, "regex", e.regex), T(this, A, L).call(this, "invalidErrorMessage", e.invalidErrorMessage), T(this, A, L).call(this, "settings", e.settings), T(this, A, L).call(this, "condition", e.condition);
    const o = this.workspaceContext.getContainerIndexPathForField(this.field.id);
    this.workspaceContext.moveField(
      this.field.id,
      o,
      e.containerIndexPath
    ), await T(this, za, ou).call(this, e.prevalueSourceId, e.prevalues);
  };
  await this.editNewOrExistingField(this.field, !1, t);
};
al = /* @__PURE__ */ new WeakSet();
zh = function() {
  var t;
  (t = this.structureManager) == null || t.deleteQuestion(
    this.field,
    this.fieldsetId,
    this.pageId
  );
};
sl = /* @__PURE__ */ new WeakSet();
Nh = function() {
  var t;
  (t = this.structureManager) == null || t.copyQuestion(
    this.field,
    this.fieldsetId,
    this.pageId
  );
};
nl = /* @__PURE__ */ new WeakSet();
qh = function(t) {
  if (this.field) {
    const e = "caption";
    t.target.name === e && T(this, A, L).call(this, e, t.target.value.toString());
  }
};
ll = /* @__PURE__ */ new WeakSet();
Vh = function() {
  this._aliasLocked = !this._aliasLocked;
};
cl = /* @__PURE__ */ new WeakSet();
Bh = function(t) {
  if (this.field) {
    const e = "alias";
    t.target.name === e && T(this, A, L).call(this, e, t.target.value.toString());
  }
};
ul = /* @__PURE__ */ new WeakSet();
jh = function(t) {
  if (t instanceof ry && this.field) {
    const e = t.composedPath()[0], o = "tooltip";
    e.name === o && T(this, A, L).call(this, o, e.value.toString());
  }
};
A = /* @__PURE__ */ new WeakSet();
L = function(t, e) {
  this.workspaceContext.setFieldProperty(
    this.pageIndex,
    this.fieldsetIndex,
    this.containerIndex,
    this.index,
    t,
    e
  );
};
dl = /* @__PURE__ */ new WeakSet();
Gh = function(t) {
  const e = this.allFieldTypes.find(
    (o) => o.id == t.fieldTypeId
  );
  if (e)
    return n`<div id="fieldType">
      <div id="tags">
        <uui-tag look="default">${e.name}</uui-tag>
        ${m(
      t.mandatory,
      () => n`<uui-tag look="default"
              ><span
                >* ${this.localize.term("general_mandatory")}</span
              ></uui-tag
            >`
    )}
        ${m(
      t.containsSensitiveData,
      () => n`<uui-tag look="default"
              ><span
                ><uui-icon name="icon-lock"></uui-icon>${this.localize.term("fieldSettings_sensitiveData")}</span
              ></uui-tag
            >`
    )}
      </div>
      ${T(this, iu, Hh).call(this, t, e)}
    </div>`;
};
iu = /* @__PURE__ */ new WeakSet();
Hh = function(t, e) {
  return Td.getByAlias(
    e.previewView
  ) ? n`<umb-extension-slot
        type="formsFieldPreview"
        .filter=${(r) => r.alias === e.previewView}
        .props=${{ settings: t.settings, prevalues: this._fieldPrevalues }}
      >
      </umb-extension-slot>` : n``;
};
ye.styles = [
  C`
      :host {
        display: block;
        border-bottom: 1px solid #e9e9eb;
        padding: var(--uui-size-layout-1) 0;
      }

      #fieldHeader {
        display: flex;
        margin-bottom: var(--uui-size-3);
      }

      uui-action-bar {
        margin-left: var(--uui-size-3);
      }

      #caption {
        flex: 1;
        margin-right: var(--uui-size-3);
      }

      #help {
        width: 300px;
      }

      #fieldContent {
        display: flex;
      }

      #fieldType {
        flex: 1 1 auto;
        background-color: var(--uui-color-surface-alt);
        margin-left: var(--uui-size-3);
        padding: var(--uui-size-3);
      }

      #tags {
        margin-bottom:var(--uui-size-3);
        display: flex;
				gap: var(--uui-size-space-2);
      }

      :host([sort-mode-active]) {
        position: relative;
        display: flex;
        padding: 0;
        margin-bottom: var(--uui-size-3);
      }

      :host([sort-mode-active]:last-of-type) {
        margin-bottom: 0;
      }

      :host([sort-mode-active]:not([inherited])) {
        cursor: grab;
      }

      :host([sort-mode-active]) .sortable {
        flex: 1;
        display: flex;
        background-color: var(--uui-color-divider);
        align-items: center;
        padding: 0 var(--uui-size-3);
        gap: var(--uui-size-3);
      }

      :host([sort-mode-active]) uui-input {
        max-width: 75px;
      }

      /* Placeholder style, used when property is being dragged.*/
      :host(.--umb-sorter-placeholder) > * {
        visibility: hidden;
      }

      :host(.--umb-sorter-placeholder)::after {
        content: "";
        inset: 0;
        position: absolute;
        border: 1px dashed var(--uui-color-divider-emphasis);
        border-radius: var(--uui-border-radius);
      }

      uui-tag uui-icon {
        font-size: xx-small;
        margin-right: 4px;
      }
    `
];
Ue([
  p({ type: Object })
], ye.prototype, "field", 2);
Ue([
  p()
], ye.prototype, "fieldsetId", 2);
Ue([
  p()
], ye.prototype, "pageId", 2);
Ue([
  p({ type: Number })
], ye.prototype, "pageIndex", 2);
Ue([
  p({ type: Number })
], ye.prototype, "fieldsetIndex", 2);
Ue([
  p({ type: Number })
], ye.prototype, "containerIndex", 2);
Ue([
  w()
], ye.prototype, "_aliasLocked", 2);
Ue([
  w()
], ye.prototype, "_fieldPrevalues", 2);
ye = Ue([
  h(nC)
], ye);
var lC = Object.defineProperty, cC = Object.getOwnPropertyDescriptor, Na = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? cC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && lC(e, o, i), i;
}, uC = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, ko = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, gt = (t, e, o) => (uC(t, e, "access private method"), o), ml, Yh, pl, Kh, hl, Xh, fl, Qh, fi, Xr;
const dC = "forms-form-fieldset";
let _o = class extends Ee {
  constructor() {
    super(...arguments), ko(this, ml), ko(this, pl), ko(this, hl), ko(this, fl), ko(this, fi), this.pageIndex = 0, this.fieldTypes = [];
  }
  render() {
    var t;
    return n`<uui-box
      ><div slot="header">
        ${m(
      !this.sortModeActive,
      () => {
        var e;
        return n` <uui-input
            .value=${(e = this.fieldset) == null ? void 0 : e.caption}
            label="caption"
            .placeholder=${this.localize.term("formEdit_groupTitlePlaceholder")}
            @change=${gt(this, fl, Qh)}
          ></uui-input>`;
      },
      () => {
        var e;
        return n`<uui-icon name="icon-navigation"></uui-icon>
          ${((e = this.fieldset) == null ? void 0 : e.caption) ?? this.localize.term("formEdit_groupTitlePlaceholder")}
        </div>`;
      }
    )}
      </div>
      ${m(
      !this.sortModeActive,
      () => n`<div slot="header-actions">
          <uui-action-bar>
            <uui-button
              label="edit"
              look="secondary"
              color="default"
              @click=${gt(this, ml, Yh)}
            >
              <uui-icon name="settings"></uui-icon>
            </uui-button>
            <uui-button
              label="copy"
              look="secondary"
              color="default"
              @click=${gt(this, hl, Xh)}
            >
              <uui-icon name="copy"></uui-icon>
            </uui-button>
            <uui-button
              label=${this.localize.term("general_delete")}
              look="secondary"
              color="default"
              @click=${gt(this, pl, Kh)}
            >
              <uui-icon name="delete"></uui-icon>
            </uui-button>
          </uui-action-bar>
        </div>`
    )}
      ${m(
      !this.sortModeActive,
      () => {
        var e, o;
        return n` <forms-form-condition-summary
          formElement="fieldset"
          .condition=${((e = this.fieldset) == null ? void 0 : e.condition) ?? void 0}
          .operatorTypes=${((o = this.workspaceContext) == null ? void 0 : o.getConditionOperators) ?? []}
          .allFields=${this.allFields}
        >
        </forms-form-condition-summary>`;
      }
    )}

      <div id="fieldsetContainers">
        ${(t = this.fieldset) == null ? void 0 : t.containers.map(
      (e, o) => n`<forms-form-fieldset-container
              .fieldset=${this.fieldset}
              .container=${e}
              .index=${o}
              .fieldsetIndex=${this.index}
              .pageIndex=${this.pageIndex}
              .allFields=${this.allFields}
              .allFieldTypes=${this.allFieldTypes}
              .sortModeActive=${this.sortModeActive}
            ></forms-form-fieldset-container>`
    )}
      </div>
    </uui-box>`;
  }
};
ml = /* @__PURE__ */ new WeakSet();
Yh = async function() {
  if (!this.modalContext || !this.workspaceContext || !this.fieldset)
    return;
  const t = this.workspaceContext.getAllPages(), e = this.pageIndex, o = this.fieldset.condition ? structuredClone(this.fieldset.condition) : ot.getConditionScaffold(), r = structuredClone(this.fieldset.containers), a = await this.modalContext.open(
    this,
    xg,
    {
      data: {
        fields: this.allFields,
        pages: t
      },
      value: {
        condition: o,
        containers: r,
        pageIndex: e
      }
    }
  ).onSubmit().catch(() => {
  });
  a && (this.fieldset && this.workspaceContext.moveFieldSet(
    this.pageIndex,
    this.index,
    a.pageIndex
  ), gt(this, fi, Xr).call(this, "condition", a.condition), gt(this, fi, Xr).call(this, "containers", a.containers));
};
pl = /* @__PURE__ */ new WeakSet();
Kh = function() {
  var t;
  (t = this.structureManager) == null || t.deleteFormGroup(this.fieldset);
};
hl = /* @__PURE__ */ new WeakSet();
Xh = function() {
  var t;
  (t = this.structureManager) == null || t.copyFormGroup(this.fieldset);
};
fl = /* @__PURE__ */ new WeakSet();
Qh = function(t) {
  if (this.fieldset) {
    const e = t.target.value.toString();
    gt(this, fi, Xr).call(this, "caption", e);
  }
};
fi = /* @__PURE__ */ new WeakSet();
Xr = function(t, e) {
  this.workspaceContext.setFieldsetProperty(
    this.pageIndex,
    this.index,
    t,
    e
  );
};
_o.styles = [
  C`
      uui-input {
        flex: 1;
      }

      #header {
        display: flex;
        align-items: center;
        column-gap: var(--uui-size-2);
      }

      #fieldsetContainers {
        display: flex;
        column-gap: var(--uui-size-space-5);
      }

      forms-form-fieldset-container {
        box-sizing: border-box;
        display: flex;
        flex: 1;
      }
    `
];
Na([
  p({ type: Object })
], _o.prototype, "fieldset", 2);
Na([
  p({ type: Number })
], _o.prototype, "pageIndex", 2);
Na([
  p({ type: Array })
], _o.prototype, "fieldTypes", 2);
_o = Na([
  h(dC)
], _o);
var mC = Object.defineProperty, pC = Object.getOwnPropertyDescriptor, Oo = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? pC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && mC(e, o, i), i;
}, ru = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Fr = (t, e, o) => (ru(t, e, "read from private field"), o ? o.call(t) : e.get(t)), Do = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, yl = (t, e, o, r) => (ru(t, e, "write to private field"), e.set(t, o), o), ts = (t, e, o) => (ru(t, e, "access private method"), o), Ot, qa, gl, Jh, _l, Zh, vl, ef;
const hC = "forms-form-fieldset-container";
let rt = class extends Ee {
  constructor() {
    super(...arguments), Do(this, gl), Do(this, _l), Do(this, vl), this.pageIndex = 0, this.fieldsetIndex = 0, this.fieldTypes = [], Do(this, Ot, []), Do(this, qa, void 0);
  }
  sortModeEnabled() {
    ts(this, gl, Jh).call(this), ts(this, _l, Zh).call(this);
  }
  render() {
    var t, e, o;
    return n`<div
      class="fieldset-container fieldset-container-${(t = this.container) == null ? void 0 : t.id} ${(e = this.container) != null && e.fields.length ? "" : "empty"}"
    >
      ${(o = this.container) == null ? void 0 : o.fields.map(
      (r, i) => {
        var a;
        return n`<forms-form-field
            class="container-${(a = this.container) == null ? void 0 : a.id}"
            sort-unique=${r.id}
            .field=${r}
            .fieldsetId=${this.fieldset.id}
            .pageId=${this.fieldset.page}
            .pageIndex=${this.pageIndex}
            .fieldsetIndex=${this.fieldsetIndex}
            .containerIndex=${this.index}
            .index=${i}
            .allFields=${this.allFields}
            .allFieldTypes=${this.allFieldTypes}
            .sortModeActive=${this.sortModeActive}
          ></forms-form-field>`;
      }
    )}
      ${m(
      !this.sortModeActive,
      () => n` <uui-button
          class="add-question-button"
          @click=${ts(this, vl, ef)}
          look="secondary"
          label=${this.localize.term("formEdit_addQuestion")}
        ></uui-button>`
    )}
    </div>`;
  }
};
Ot = /* @__PURE__ */ new WeakMap();
qa = /* @__PURE__ */ new WeakMap();
gl = /* @__PURE__ */ new WeakSet();
Jh = function() {
  var t, e;
  yl(this, qa, new ji(this, {
    ...new Yi(
      "Forms.SorterIdentifier.Field",
      `forms-form-field.container-${(t = this.container) == null ? void 0 : t.id}`,
      `.fieldset-container-${(e = this.container) == null ? void 0 : e.id}`
    ).config,
    onChange: ({ model: o }) => {
      yl(this, Ot, o);
    },
    onEnd: () => {
      var o;
      (o = this.structureManager) == null || o.reorderQuestions(
        this.fieldset,
        this.container,
        Fr(this, Ot)
      );
    }
  }));
};
_l = /* @__PURE__ */ new WeakSet();
Zh = function() {
  var e;
  yl(this, Ot, []);
  const t = this.container.fields;
  for (let o = 0; o < t.length; o++) {
    const r = t[o];
    Fr(this, Ot).push(r.id);
  }
  (e = Fr(this, qa)) == null || e.setModel(Fr(this, Ot));
};
vl = /* @__PURE__ */ new WeakSet();
ef = function() {
  this.modalContext.open(
    this,
    Bd
  ).onSubmit().then(async (e) => {
    if (!e.selectedValue)
      return;
    if (!this.workspaceContext)
      throw new Error("No workspace context");
    const o = ot.getQuestionScaffold();
    o.fieldTypeId = e.selectedValue.id;
    const r = async (i) => {
      var a;
      o.caption = i.caption, o.alias = i.alias, o.tooltip = i.tooltip, o.fieldTypeId = i.fieldType.id, o.containsSensitiveData = i.containsSensitiveData, o.allowedUploadTypes = i.allowedUploadTypes, o.allowMultipleFileUploads = i.allowMultipleFileUploads, o.preValues = i.prevalues, o.prevalueSourceId = i.prevalueSourceId || Dt, o.mandatory = i.mandatory, o.requiredErrorMessage = i.requiredErrorMessage, o.regex = i.regex, o.invalidErrorMessage = i.invalidErrorMessage, o.settings = i.settings, o.condition = i.condition, (a = this.structureManager) == null || a.insertQuestion(
        o,
        this.fieldset,
        this.container
      );
    };
    await this.editNewOrExistingField(o, !0, r);
  }).catch(() => {
  });
};
rt.styles = [
  C`
      uui-input,
      .fieldset-container {
        flex: 1;
      }

      .fieldset-container + .fieldset-container {
        border-left: 1px solid var(--uui-color-divider-standalone);
      }

      .add-question-button {
        margin-top: var(--add-button-margin, var(--uui-size-3));
      }

      .empty {
        --add-button-margin: 0;

        display: flex;
        justify-content: center;
        align-items: center;
        flex: 1;
        border: 2px dashed var(--uui-color-divider-standalone);
      }
    `
];
Oo([
  p({ type: Object })
], rt.prototype, "fieldset", 2);
Oo([
  p({ type: Object })
], rt.prototype, "container", 2);
Oo([
  p({ type: Number })
], rt.prototype, "pageIndex", 2);
Oo([
  p({ type: Number })
], rt.prototype, "fieldsetIndex", 2);
Oo([
  p({ type: Array })
], rt.prototype, "fieldTypes", 2);
rt = Oo([
  h(hC)
], rt);
var fC = Object.defineProperty, yC = Object.getOwnPropertyDescriptor, Va = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? yC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && fC(e, o, i), i;
}, gC = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, _C = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, vC = (t, e, o) => (gC(t, e, "access private method"), o), Sl, tf;
const SC = "forms-form-workflow-summary";
let vo = class extends Ee {
  constructor() {
    super(...arguments), _C(this, Sl), this.manualApproval = !1;
  }
  render() {
    var t, e, o;
    return n`<uui-box>
      <div class="flex">
        <div>
          <forms-form-workflow-summary-stage
            .workflows=${((t = this.workflows) == null ? void 0 : t.onSubmit) ?? []}
            .submitMessageDetail=${this.submitMessageDetail}
            .allFields=${this.allFields}
            .label=${this.localize.term("formWorkflows_onSubmit")}
            collectionName="onSubmit"
            icon="icon-check"
          ></forms-form-workflow-summary-stage>
          ${m(
      (((e = this.workflows) == null ? void 0 : e.onApprove) ?? []).length > 0,
      () => {
        var r;
        return n`<forms-form-workflow-summary-stage
              .workflows=${((r = this.workflows) == null ? void 0 : r.onApprove) ?? []}
              .allFields=${this.allFields}
              .label=${this.localize.term("formWorkflows_onApprove") + (this.manualApproval ? " (" + this.localize.term("formWorkflows_automatic") + ")" : "")}
              collectionName="onApprove"
              icon="icon-thumb-up"
            ></forms-form-workflow-summary-stage>`;
      }
    )}
          ${m(
      this.manualApproval && (((o = this.workflows) == null ? void 0 : o.onReject) ?? []).length > 0,
      () => {
        var r;
        return n`<forms-form-workflow-summary-stage
              .workflows=${((r = this.workflows) == null ? void 0 : r.onReject) ?? []}
              .allFields=${this.allFields}
              .label=${this.localize.term("formWorkflows_onReject")}
              collectionName="onReject"
              icon="icon-delete"
            ></forms-form-workflow-summary-stage>`;
      }
    )}
        </div>
        <uui-button
          @click=${vC(this, Sl, tf)}
          look="secondary"
          label=${this.localize.term("formWorkflows_configureWorkflow")}
        ></uui-button>
      </div>
    </uui-box>`;
  }
};
Sl = /* @__PURE__ */ new WeakSet();
tf = function() {
  if (!this.modalContext)
    return;
  this.modalContext.open(
    this,
    Pg,
    {
      data: {
        manualApproval: this.manualApproval,
        fields: this.allFields
      },
      value: {
        workflows: this.workflows,
        submitMessageDetail: this.submitMessageDetail
      }
    }
  ).onSubmit().then((e) => {
    e.submitMessageDetail && (this.workspaceContext.setFormProperty(
      "messageOnSubmit",
      e.submitMessageDetail.messageOnSubmit
    ), this.workspaceContext.setFormProperty(
      "messageOnSubmitIsHtml",
      e.submitMessageDetail.messageOnSubmitIsHtml
    ), this.workspaceContext.setFormProperty(
      "goToPageOnSubmit",
      e.submitMessageDetail.goToPageOnSubmit
    )), e.workflows && this.workspaceContext.setFormProperty(
      "formWorkflows",
      e.workflows
    );
  }).catch(() => {
  });
};
vo.styles = [
  C`
      .flex {
        display: flex;
        column-gap: var(--uui-size-5);
      }

      .flex > div:first-child {
        flex: 1;
      }

      uui-button {
        align-self:center;
      }

      forms-form-workflow-summary-stage + forms-form-workflow-summary-stage {
        margin-top: var(--uui-size-5);
      }
    `
];
Va([
  p({ type: Object })
], vo.prototype, "workflows", 2);
Va([
  p({ type: Object })
], vo.prototype, "submitMessageDetail", 2);
Va([
  p({ type: Boolean })
], vo.prototype, "manualApproval", 2);
vo = Va([
  h(SC)
], vo);
var wC = Object.defineProperty, bC = Object.getOwnPropertyDescriptor, Po = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? bC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && wC(e, o, i), i;
}, FC = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, od = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, id = (t, e, o) => (FC(t, e, "access private method"), o), wl, of, bl, rf;
const EC = "forms-form-workflow-summary-stage";
let at = class extends Ee {
  constructor() {
    super(...arguments), od(this, wl), od(this, bl), this.workflows = [], this.collectionName = "", this.label = "", this.icon = "";
  }
  render() {
    var t;
    return n` <uui-icon .name=${this.icon}></uui-icon>
      <span>${this.label}</span>
      <uui-icon name="icon-arrow-right"></uui-icon>
      ${m(
      this.submitMessageDetail,
      () => n`<uui-button
            compact
            look="outline"
            color="positive"
            .label=${this.localize.term("formWorkflows_submitMessage")}
            @click=${id(this, wl, of)}
          ></uui-button>`
    )}
      ${(t = this.workflows) == null ? void 0 : t.map(
      (e, o) => n` ${m(
        this.submitMessageDetail || o > 0,
        () => n`<span>and</span>`
      )}
            <uui-button
              compact
              label=${e.name}
              look=${e.active ? "outline" : "placeholder"}
              color=${e.active ? "positive" : "default"}
              @click=${() => id(this, bl, rf).call(this, o)}
            ></uui-button>`
    )}`;
  }
};
wl = /* @__PURE__ */ new WeakSet();
of = async function() {
  var r, i, a;
  if (!this.modalContext || !this.workspaceContext)
    return;
  const t = await this.workspaceContext.getRichTextConfiguration(), o = await this.modalContext.open(
    this,
    Jd,
    {
      data: {
        richTextConfiguration: t
      },
      value: {
        messageOnSubmit: (r = this.submitMessageDetail) == null ? void 0 : r.messageOnSubmit,
        messageOnSubmitIsHtml: ((i = this.submitMessageDetail) == null ? void 0 : i.messageOnSubmitIsHtml) || !1,
        goToPageOnSubmit: (a = this.submitMessageDetail) == null ? void 0 : a.goToPageOnSubmit
      }
    }
  ).onSubmit().catch(() => {
  });
  o && (this.workspaceContext.setFormProperty(
    "messageOnSubmit",
    o.messageOnSubmit
  ), this.workspaceContext.setFormProperty(
    "messageOnSubmitIsHtml",
    o.messageOnSubmitIsHtml
  ), this.workspaceContext.setFormProperty(
    "goToPageOnSubmit",
    o.goToPageOnSubmit
  ));
};
bl = /* @__PURE__ */ new WeakSet();
rf = async function(t) {
  if (!this.modalContext)
    return;
  const e = this.workflows[t], o = await this.workspaceContext.loadWorkflowType(
    e.workflowTypeId
  );
  if (!o)
    throw new Error(
      `Workflow type with id ${e.workflowTypeId} could not be found.`
    );
  const i = await this.modalContext.open(
    this,
    em,
    {
      data: {
        fields: this.allFields,
        workflowType: o,
        isNew: !1
      },
      value: {
        name: e.name,
        active: e.active,
        includeSensitiveData: e.includeSensitiveData === Je.TRUE,
        collectionName: this.collectionName,
        settings: e.settings,
        condition: e.condition
      }
    }
  ).onSubmit().catch(() => {
  });
  !this.workspaceContext || !i || (this.workspaceContext.setWorkflowProperty(
    this.collectionName,
    t,
    "name",
    i.name
  ), this.workspaceContext.setWorkflowProperty(
    this.collectionName,
    t,
    "active",
    i.active
  ), this.workspaceContext.setWorkflowProperty(
    this.collectionName,
    t,
    "includeSensitiveData",
    i.includeSensitiveData ? Je.TRUE : Je.FALSE
  ), this.workspaceContext.setWorkflowProperty(
    this.collectionName,
    t,
    "settings",
    i.settings
  ), this.workspaceContext.setWorkflowProperty(
    this.collectionName,
    t,
    "condition",
    i.condition
  ), i.collectionName != this.collectionName && this.workspaceContext.moveWorkflow(
    t,
    this.collectionName,
    i.collectionName
  ));
};
at.styles = [
  C`
      :host {
        display: flex;
        align-items: center;
        column-gap: var(--uui-size-3);
      }

      uui-icon:first-of-type {
        border: 1px solid var(--uui-color-border-standalone);
        border-radius: 50%;
        padding: var(--uui-size-2);
        width: var(--uui-size-6);
        height: var(--uui-size-6);
      }
    `
];
Po([
  p({ type: Array })
], at.prototype, "workflows", 2);
Po([
  p()
], at.prototype, "collectionName", 2);
Po([
  p({ type: Object })
], at.prototype, "submitMessageDetail", 2);
Po([
  p()
], at.prototype, "label", 2);
Po([
  p()
], at.prototype, "icon", 2);
at = Po([
  h(EC)
], at);
var TC = Object.defineProperty, $C = (t, e, o, r) => {
  for (var i = void 0, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = s(e, o, i) || i);
  return i && TC(e, o, i), i;
}, $t, ua, af, da, sf;
const ku = class ku extends st(
  It
) {
  constructor() {
    super();
    f(this, ua);
    f(this, da);
    f(this, $t, void 0);
    this.consumeContext(Ae, (o) => {
      o && (_(this, $t, o), F(this, ua, af).call(this));
    });
  }
  onObserveForm() {
  }
  getAllFieldsForForm() {
    var o;
    return (o = d(this, $t)) == null ? void 0 : o.getAllFields();
  }
  onInputChange(o, r) {
    const i = o.composedPath()[0];
    this.setPropertyValue(r, i.value.toString());
  }
  onRadioChange(o, r) {
    this.setPropertyValue(r, o.target.value);
  }
  onSelectChange(o, r) {
    this.setPropertyValue(r, o.target.value);
  }
  setPropertyValue(o, r) {
    var i;
    (i = d(this, $t)) == null || i.setFormProperty(o, r), this.dispatchEvent(new CustomEvent("valueChange"));
  }
  renderToggle(o, r, i) {
    const a = this.localize.term(i);
    return n`<uui-toggle
      ?checked=${o}
      .label=${a}
      @change=${(s) => F(this, da, sf).call(this, s, r)}
    ></uui-toggle>`;
  }
};
$t = new WeakMap(), ua = new WeakSet(), af = function() {
  this.observe(d(this, $t).data, (o) => {
    o && (this.form = o, this.onObserveForm());
  });
}, da = new WeakSet(), sf = function(o, r) {
  this.setPropertyValue(r, o.target.checked);
}, ku.styles = [
  C`
      .flex {
        display: flex;
        flex-direction: column;
      }

      .flex + .flex {
        margin-top: var(--uui-size-3);
      }

      .flex.gap {
        margin-top: var(--uui-size-5);
      }

      .flex + uui-toggle {
        display: block;
        margin-top: var(--uui-size-3);
      }
    `
];
let re = ku;
$C([
  w()
], re.prototype, "form");
var CC = Object.defineProperty, OC = Object.getOwnPropertyDescriptor, PC = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? OC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && CC(e, o, i), i;
};
const kC = "forms-settings-store-records";
let rd = class extends re {
  constructor() {
    super(...arguments), this.alias = "storeRecordsLocally";
  }
  render() {
    return n` <umb-property-layout
      .alias=${this.alias}
      .label=${this.localize.term("formSettings_storeRecords")}
      .description=${this.localize.term("formSettings_storeRecordsDescription")}
    >
      <div slot="editor">
        ${this.renderToggle(
      this.form.storeRecordsLocally,
      this.alias,
      "formSettings_storeRecordsConfirm"
    )}
      </div>
    </umb-property-layout>`;
  }
};
rd = PC([
  h(kC)
], rd);
var DC = Object.defineProperty, xC = Object.getOwnPropertyDescriptor, MC = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? xC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && DC(e, o, i), i;
};
const AC = "forms-settings-captions";
let ad = class extends re {
  render() {
    var t, e, o;
    return n` <umb-property-layout
      alias="captions"
      .label=${this.localize.term("formSettings_captions")}
      .description=${this.localize.term("formSettings_captionsDescription")}
    >
      <div slot="editor">
        <div class="flex">
          <uui-label
            >${this.localize.term(
      "formSettings_captionSubmitButton"
    )}</uui-label
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_captionSubmitButton")}
            .value=${(t = this.form) == null ? void 0 : t.submitLabel}
            @change=${(r) => this.onInputChange(r, "submitLabel")}
          ></uui-input>
        </div>
        <div class="flex">
          <uui-label
            >${this.localize.term("formSettings_captionNextButton")}</uui-label
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_captionNextButton")}
            .value=${(e = this.form) == null ? void 0 : e.nextLabel}
            @change=${(r) => this.onInputChange(r, "nextLabel")}
          ></uui-input>
        </div>
        <div class="flex">
          <uui-label
            >${this.localize.term(
      "formSettings_captionPreviousButton"
    )}</uui-label
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_captionPreviousButton")}
            .value=${(o = this.form) == null ? void 0 : o.prevLabel}
            @change=${(r) => this.onInputChange(r, "prevLabel")}
          ></uui-input>
        </div>
      </div>
    </umb-property-layout>`;
  }
};
ad = MC([
  h(AC)
], ad);
var RC = Object.defineProperty, IC = Object.getOwnPropertyDescriptor, UC = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? IC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && RC(e, o, i), i;
};
const WC = "forms-settings-styling";
let sd = class extends re {
  render() {
    var t, e;
    return n` <umb-property-layout
      alias="styling"
      .label=${this.localize.term("formSettings_styling")}
      .description=${this.localize.term("formSettings_stylingDescription")}
    >
      <div slot="editor">
        <div class="flex">
          <uui-label
            >${this.localize.term("formSettings_formCssClass")}</uui-label
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_formCssClass")}
            .value=${((t = this.form) == null ? void 0 : t.cssClass) ?? ""}
            @change=${(o) => this.onInputChange(o, "cssClass")}
          ></uui-input>
        </div>
        ${this.renderToggle(
      (e = this.form) == null ? void 0 : e.disableDefaultStylesheet,
      "disableDefaultStylesheet",
      "formSettings_disableDefaultStylesheet"
    )}
      </div>
    </umb-property-layout>`;
  }
};
sd = UC([
  h(WC)
], sd);
var LC = Object.defineProperty, zC = Object.getOwnPropertyDescriptor, NC = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? zC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && LC(e, o, i), i;
}, qC = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, VC = (t, e, o) => (qC(t, e, "read from private field"), o ? o.call(t) : e.get(t)), BC = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Fl;
const jC = "forms-settings-validation";
let nd = class extends re {
  constructor() {
    super(...arguments), BC(this, Fl, [
      {
        label: this.localize.term("formSettings_markFieldsNoIndicator"),
        value: "NoIndicator"
      },
      {
        label: this.localize.term("formSettings_markMandatoryFields"),
        value: "MarkMandatoryFields"
      },
      {
        label: this.localize.term("formSettings_markOptionalFields"),
        value: "MarkOptionalFields"
      }
    ]);
  }
  render() {
    var t, e, o, r, i, a;
    return n` <umb-property-layout
      alias="validation"
      .label=${this.localize.term("formSettings_validation")}
      .description=${this.localize.term("formSettings_validationDescription")}
    >
      <div slot="editor">
        <div class="flex">
          <uui-label
            >${this.localize.term(
      "formSettings_mandatoryErrorMessage"
    )}</uui-label
          >
          <small
            >${this.localize.term(
      "formSettings_mandatoryErrorMessageDescription"
    )}</small
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_mandatoryErrorMessage")}
            .value=${((t = this.form) == null ? void 0 : t.requiredErrorMessage) ?? ""}
            @change=${(s) => this.onInputChange(s, "requiredErrorMessage")}
          ></uui-input>
        </div>
        <div class="flex gap">
          <uui-label
            >${this.localize.term(
      "formSettings_invalidErrorMessage"
    )}</uui-label
          >
          <small
            >${this.localize.term(
      "formSettings_invalidErrorMessageDescription"
    )}</small
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_invalidErrorMessage")}
            .value=${((e = this.form) == null ? void 0 : e.invalidErrorMessage) ?? ""}
            @change=${(s) => this.onInputChange(s, "invalidErrorMessage")}
          ></uui-input>
        </div>
        ${this.renderToggle(
      (o = this.form) == null ? void 0 : o.showValidationSummary,
      "showValidationSummary",
      "formSettings_showValidationSummary"
    )}
        ${this.renderToggle(
      (r = this.form) == null ? void 0 : r.hideFieldValidation,
      "hideFieldValidation",
      "formSettings_hideFieldValidationLabels"
    )}
        <div class="flex gap">
          <uui-label
            >${this.localize.term("formSettings_markFields")}</uui-label
          >
          <uui-radio-group
            .value=${(i = this.form) == null ? void 0 : i.fieldIndicationType}
            @change=${(s) => this.onRadioChange(s, "fieldIndicationType")}
          >
            ${VC(this, Fl).map(
      (s) => n`<uui-radio
                  name=${"fieldIndicationType"}
                  value=${s.value}
                  label=${s.label}
                ></uui-radio>`
    )}
          </uui-radio-group>
        </div>
        <div class="flex gap">
          <uui-label>${this.localize.term("formSettings_indicator")}</uui-label>
          <small
            >${this.localize.term("formSettings_changeIndicatorSymbol")}</small
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_indicator")}
            .value=${((a = this.form) == null ? void 0 : a.indicator) ?? ""}
            @change=${(s) => this.onInputChange(s, "indicator")}
          ></uui-input>
        </div>
      </div>
    </umb-property-layout>`;
  }
};
Fl = /* @__PURE__ */ new WeakMap();
nd = NC([
  h(jC)
], nd);
var GC = Object.defineProperty, HC = Object.getOwnPropertyDescriptor, YC = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? HC(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && GC(e, o, i), i;
}, KC = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, XC = (t, e, o) => (KC(t, e, "read from private field"), o ? o.call(t) : e.get(t)), QC = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, El;
const JC = "forms-settings-autocomplete";
let ld = class extends re {
  constructor() {
    super(...arguments), QC(this, El, [
      {
        label: this.localize.term("formSettings_autocompleteNone"),
        value: "None"
      },
      {
        label: this.localize.term("formSettings_autocompleteOn"),
        value: "On"
      },
      {
        label: this.localize.term("formSettings_autocompleteOff"),
        value: "Off"
      }
    ]);
  }
  render() {
    return n` <umb-property-layout
      alias="autocomplete"
      .label=${this.localize.term("formSettings_autocomplete")}
      .description=${this.localize.term("formSettings_autocompleteDescription")}
    >
      <div slot="editor" class="flex">
        <uui-label
          >${this.localize.term(
      "formSettings_autocompleteAttributeValue"
    )}</uui-label
        >
        <uui-radio-group
            .value=${this.form.autocompleteAttribute}
            @change=${(t) => this.onRadioChange(t, "autoCompleteAttribute")}
          >
            ${XC(this, El).map(
      (t) => n`<uui-radio
                  name=${"autoCompleteAttribute"}
                  value=${t.value}
                  label=${t.label}
                ></uui-radio>`
    )}
          </uui-radio-group>
      </div>
    </umb-property-layout>`;
  }
};
El = /* @__PURE__ */ new WeakMap();
ld = YC([
  h(JC)
], ld);
var ZC = Object.defineProperty, eO = Object.getOwnPropertyDescriptor, tO = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? eO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && ZC(e, o, i), i;
};
const oO = "forms-settings-moderation";
let cd = class extends re {
  constructor() {
    super(...arguments), this.alias = "manualApproval";
  }
  render() {
    var t;
    return n` <umb-property-layout
      .alias=${this.alias}
      .label=${this.localize.term("formSettings_moderation")}
      .description=${this.localize.term("formSettings_moderationDescription")}
    >
      <div slot="editor">
        ${this.renderToggle(
      (t = this.form) == null ? void 0 : t.manualApproval,
      this.alias,
      "formSettings_enablePostModeration"
    )}
      </div>
    </umb-property-layout>`;
  }
};
cd = tO([
  h(oO)
], cd);
var iO = Object.defineProperty, rO = Object.getOwnPropertyDescriptor, aO = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? rO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && iO(e, o, i), i;
}, nf = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, sO = (t, e, o) => (nf(t, e, "read from private field"), e.get(t)), nO = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, lO = (t, e, o, r) => (nf(t, e, "write to private field"), e.set(t, o), o), Er;
const cO = "forms-settings-data-retention";
let Tl = class extends re {
  constructor() {
    super(), nO(this, Er, !1), this.consumeContext(Qi, (t) => {
      t && this.observe(t.config, (e) => {
        e && lO(this, Er, e == null ? void 0 : e.scheduledRecordDeletionEnabled);
      });
    });
  }
  render() {
    return n`<umb-property-layout
      alias="dataRetention"
      .label=${this.localize.term("formSettings_dataRetention")}
      .description=${this.localize.term("formSettings_dataRetentionDescription")}>

      <div slot="editor">

        <forms-settings-data-retention-stage
          stage="Submitted">
        </forms-settings-data-retention-stage>
        <forms-settings-data-retention-stage
          stage="Approved">
        </forms-settings-data-retention-stage>
        <forms-settings-data-retention-stage
          stage="Rejected">
        </forms-settings-data-retention-stage>

        ${m(
      !sO(this, Er),
      () => n`<div class="note">
            ${this.localize.term("formSettings_scheduledRecordDeletionNotEnabled")}</div>`
    )}


      </div>

    </umb-property-layout>`;
  }
};
Er = /* @__PURE__ */ new WeakMap();
Tl.styles = [
  C`
      .note {
        font-style: italic;
        margin-top: 10px;
      }
    `
];
Tl = aO([
  h(cO)
], Tl);
var uO = Object.defineProperty, dO = Object.getOwnPropertyDescriptor, au = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? dO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && uO(e, o, i), i;
}, su = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, mO = (t, e, o) => (su(t, e, "read from private field"), o ? o.call(t) : e.get(t)), xo = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, pO = (t, e, o, r) => (su(t, e, "write to private field"), e.set(t, o), o), ke = (t, e, o) => (su(t, e, "access private method"), o), Qr, Jr, nu, Zr, $l, Kt, yi, Cl, lf;
const hO = "forms-settings-data-retention-stage";
let ea = class extends re {
  constructor() {
    super(...arguments), xo(this, Jr), xo(this, Zr), xo(this, Kt), xo(this, Cl), xo(this, Qr, 365), this._showDaysToRetain = !1, this.stage = "";
  }
  connectedCallback() {
    super.connectedCallback(), this._showDaysToRetain = this.form[ke(this, Kt, yi).call(this)] > 0;
  }
  render() {
    return n` <div>
      <uui-toggle
        ?checked=${this._showDaysToRetain}
        .label=${this.localize.term(
      `formSettings_dataRetention${this._showDaysToRetain ? "Remove" : "Retain"}${this.stage}Records`
    )}
        @change=${ke(this, Cl, lf)}
      ></uui-toggle>
      ${m(
      this._showDaysToRetain,
      () => n`<div>
          <uui-label>
            ${this.localize.term(
        `formSettings_dataRetentionFor${this.stage}Records`
      )}
          </uui-label>
          <uui-input
            type="number"
            name="numberOfDays"
            .value=${ke(this, Jr, nu).call(this)}
            @change=${(t) => this.onInputChange(t, ke(this, Kt, yi).call(this))}
            label="numberOfDays"
          ></uui-input>
        </div>`
    )}
    </div>`;
  }
};
Qr = /* @__PURE__ */ new WeakMap();
Jr = /* @__PURE__ */ new WeakSet();
nu = function() {
  return this.form[ke(this, Kt, yi).call(this)];
};
Zr = /* @__PURE__ */ new WeakSet();
$l = function(t) {
  return this.setPropertyValue(ke(this, Kt, yi).call(this), t);
};
Kt = /* @__PURE__ */ new WeakSet();
yi = function() {
  return `daysToRetain${this.stage}RecordsFor`;
};
Cl = /* @__PURE__ */ new WeakSet();
lf = function() {
  this._showDaysToRetain = !this._showDaysToRetain, this._showDaysToRetain ? ke(this, Zr, $l).call(this, mO(this, Qr)) : (pO(this, Qr, ke(this, Jr, nu).call(this)), ke(this, Zr, $l).call(this, 0));
};
au([
  w()
], ea.prototype, "_showDaysToRetain", 2);
au([
  p()
], ea.prototype, "stage", 2);
ea = au([
  h(hO)
], ea);
var fO = Object.defineProperty, yO = Object.getOwnPropertyDescriptor, cf = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? yO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && fO(e, o, i), i;
}, lu = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Pt = (t, e, o) => (lu(t, e, "read from private field"), o ? o.call(t) : e.get(t)), ve = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, uf = (t, e, o, r) => (lu(t, e, "write to private field"), e.set(t, o), o), tt = (t, e, o) => (lu(t, e, "access private method"), o), Qe, cu, Ba, Ol, df, ta, Pl, uu, mf, du, pf, mu, hf, kl, ff, Dl, yf;
const gO = "forms-settings-fields-display";
let xl = class extends re {
  constructor() {
    super(...arguments), ve(this, Ol), ve(this, ta), ve(this, uu), ve(this, du), ve(this, mu), ve(this, kl), ve(this, Dl), ve(this, Qe, []), ve(this, cu, new ji(this, {
      ...new Yi(
        "Forms.SorterIdentifier.DisplayField",
        ".display-field-row"
      ).config,
      onChange: ({ model: t }) => {
        uf(this, Qe, t);
      },
      onEnd: () => {
        const t = structuredClone(
          this.form.selectedDisplayFields
        );
        t.sort(
          (e, o) => Pt(this, Qe).indexOf(e.alias) - Pt(this, Qe).indexOf(o.alias)
        ), this.setPropertyValue("selectedDisplayFields", t);
      }
    })), this.formFields = [], ve(this, Ba, [
      {
        name: "member",
        labelKey: "formEntries_member"
      },
      {
        name: "state",
        labelKey: "formEntries_state"
      },
      {
        name: "created",
        labelKey: "formEntries_submittedOn"
      },
      {
        name: "update",
        labelKey: "formEntries_updatedOn"
      },
      {
        name: "workflows",
        labelKey: "formEdit_workflows"
      }
    ]);
  }
  onObserveForm() {
    tt(this, Ol, df).call(this);
  }
  render() {
    return n`<umb-property-layout
      alias="fieldsDisplayed"
      .label=${this.localize.term("formSettings_fieldsDisplayed")}
      .description=${this.localize.term(
      "formSettings_fieldsDisplayedDescription"
    )}
    >
      <div slot="editor">
        <div class="flex">
          ${this.renderToggle(
      this.form.displayDefaultFields,
      "displayDefaultFields",
      "formSettings_displayDefaultFields"
    )}
        </div>
        <div class="flex">
          ${m(
      this.form.displayDefaultFields,
      () => n`
                ${this.localize.term(
        "formSettings_displayDefaultFieldsDescription"
      )}
              `,
      () => n` ${tt(this, kl, ff).call(this)} ${tt(this, Dl, yf).call(this)} `
    )}
        </div>
      </div>
    </umb-property-layout>`;
  }
};
Qe = /* @__PURE__ */ new WeakMap();
cu = /* @__PURE__ */ new WeakMap();
Ba = /* @__PURE__ */ new WeakMap();
Ol = /* @__PURE__ */ new WeakSet();
df = function() {
  uf(this, Qe, []);
  const t = this.form.selectedDisplayFields;
  for (let e = 0; e < t.length; e++) {
    const o = t[e];
    Pt(this, Qe).push(o.alias);
  }
  Pt(this, cu).setModel(Pt(this, Qe));
};
ta = /* @__PURE__ */ new WeakSet();
Pl = function(t, e) {
  return this.form.selectedDisplayFields.some(
    (o) => o.alias === t && o.isSystem === e
  );
};
uu = /* @__PURE__ */ new WeakSet();
mf = function() {
  var r;
  const t = (r = this.shadowRoot) == null ? void 0 : r.getElementById(
    "fields"
  ), e = t.value.toString(), o = structuredClone(
    this.form.selectedDisplayFields
  );
  if (e.startsWith("_system_")) {
    const i = Pt(this, Ba).find(
      (a) => "_system_" + a.name === e
    );
    i && o.push({
      alias: i.name,
      caption: this.localize.term(i.labelKey),
      isSystem: !0
    });
  } else {
    const i = this.formFields.find(
      (a) => a.alias === e
    );
    i && o.push({
      alias: i.alias,
      caption: i.caption,
      isSystem: !1
    });
  }
  this.setPropertyValue("selectedDisplayFields", o), t.value = "";
};
du = /* @__PURE__ */ new WeakSet();
pf = function(t, e) {
  const o = t.target.value.toString(), r = structuredClone(
    this.form.selectedDisplayFields
  );
  r[e].caption = o || "", this.setPropertyValue("selectedDisplayFields", r);
};
mu = /* @__PURE__ */ new WeakSet();
hf = function(t) {
  const e = structuredClone(
    this.form.selectedDisplayFields
  );
  e.splice(t, 1), this.setPropertyValue("selectedDisplayFields", e);
};
kl = /* @__PURE__ */ new WeakSet();
ff = function() {
  return n`<div>
      <uui-select
        style="width:300px"
        id="fields"
        .options=${[
    ...this.formFields.filter((t) => !tt(this, ta, Pl).call(this, t.alias, !1)).map((t) => ({
      name: this.localize.term(t.caption),
      value: t.alias,
      group: this.localize.term("formSettings_formFields")
    })),
    ...Pt(this, Ba).filter((t) => !tt(this, ta, Pl).call(this, t.name, !0)).map((t) => ({
      name: this.localize.term(t.labelKey),
      value: `_system_${t.name}`,
      group: this.localize.term("formSettings_systemFields")
    }))
  ]}
      >
      </uui-select>
      <uui-button
        label=${this.localize.term("general_add")}
        look="secondary"
        color="default"
        @click=${tt(this, uu, mf)}
      ></uui-button>
    </div>`;
};
Dl = /* @__PURE__ */ new WeakSet();
yf = function() {
  return this.form.selectedDisplayFields.length === 0 ? n` ${this.localize.term("formSettings_noSelectedDisplayFields")} ` : n`<uui-table>
          <uui-table-head>
            <uui-table-head-cell></uui-table-head-cell>
            <uui-table-head-cell
              >${this.localize.term("general_alias")}</uui-table-head-cell
            >
            <uui-table-head-cell
              >${this.localize.term("general_header")}</uui-table-head-cell
            >
            <uui-table-head-cell></uui-table-head-cell>
          </uui-table-head>
          ${this.form.selectedDisplayFields.map(
    (t, e) => n`<uui-table-row
                class="display-field-row"
                sort-unique=${t.alias}
              >
                <uui-table-cell
                  ><uui-icon name="icon-navigation"></uui-icon
                ></uui-table-cell>
                <uui-table-cell>
                  ${t.alias}
                  ${m(
      t.isSystem,
      () => n`<span
                        >(${this.localize.term("general_systemField")})</span
                      >`
    )}
                </uui-table-cell>
                <uui-table-cell>
                  ${t.isSystem ? n`<uui-input
                        id="caption"
                        name="caption"
                        .value=${t.caption}
                        @change=${(o) => tt(this, du, pf).call(this, o, e)}
                        label="Caption"
                      ></uui-input>` : n`${t.caption}`}
                </uui-table-cell>
                <uui-table-cell>
                  <uui-button
                    label="Remove Field"
                    look="secondary"
                    color="default"
                    @click=${() => tt(this, mu, hf).call(this, e)}
                    ><uui-icon name="icon-trash"></uui-icon
                  ></uui-button>
                </uui-table-cell>
              </uui-table-row>`
  )}
        </uui-table>`;
};
cf([
  p({ type: Array })
], xl.prototype, "formFields", 2);
xl = cf([
  h(gO)
], xl);
var _O = Object.defineProperty, vO = Object.getOwnPropertyDescriptor, SO = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? vO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && _O(e, o, i), i;
}, wO = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, bO = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, FO = (t, e, o) => (wO(t, e, "access private method"), o), Ml, gf;
const EO = "forms-settings-multi-page";
let ud = class extends re {
  constructor() {
    super(...arguments), bO(this, Ml);
  }
  render() {
    var t, e, o, r;
    return n`<umb-property-layout
      alias="multipage"
      .label=${this.localize.term("formSettings_multiPageForms")}
      .description=${this.localize.term("formSettings_multiPageFormsDescription")}
    >
      <div slot="editor">
        <div class="flex">
          <uui-label
            >${this.localize.term(
      "formSettings_multiPageFormsShowPaging"
    )}</uui-label
          >
          <uui-select
            id="multiPageFormsShowPaging"
            .options=${FO(this, Ml, gf).call(this, this.form.showPagingOnMultiPageForms)}
            @change=${(i) => this.onSelectChange(i, "showPagingOnMultiPageForms")}
          >
          </uui-select>
        </div>
        <div class="flex">
          <uui-label
            >${this.localize.term(
      "formSettings_pagingDetailsFormat"
    )}</uui-label
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_pagingDetailsFormat")}
            .value=${(t = this.form) == null ? void 0 : t.pagingDetailsFormat}
            @change=${(i) => this.onInputChange(i, "pagingDetailsFormat")}
          ></uui-input>
        </div>
        <div class="flex">
          <uui-label
            >${this.localize.term(
      "formSettings_pageCaptionFormat"
    )}</uui-label
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_pageCaptionFormat")}
            .value=${(e = this.form) == null ? void 0 : e.pageCaptionFormat}
            @change=${(i) => this.onInputChange(i, "pageCaptionFormat")}
          ></uui-input>
        </div>
        ${this.renderToggle(
      (o = this.form) == null ? void 0 : o.showSummaryPageOnMultiPageForms,
      "showSummaryPageOnMultiPageForms",
      "formSettings_multiPageFormsShowSummaryPage"
    )}
        <div class="flex">
          <uui-label
            >${this.localize.term(
      "formSettings_summaryLabel"
    )}</uui-label
          >
          <uui-input
            type="text"
            .label=${this.localize.term("formSettings_summaryLabel")}
            .value=${(r = this.form) == null ? void 0 : r.summaryLabel}
            @change=${(i) => this.onInputChange(i, "summaryLabel")}
          ></uui-input>
        </div>
      </div>
    </umb-property-layout>`;
  }
};
Ml = /* @__PURE__ */ new WeakSet();
gf = function(t) {
  return [
    {
      name: this.localize.term("formSettings_multiPageFormsNavigationNone"),
      value: se.NONE.toString(),
      selected: t === se.NONE.toString()
    },
    {
      name: this.localize.term("formSettings_multiPageFormsNavigationShowOnTop"),
      value: se.SHOW_AT_TOP.toString(),
      selected: t === se.SHOW_AT_TOP.toString()
    },
    {
      name: this.localize.term("formSettings_multiPageFormsNavigationShowOnBottom"),
      value: se.SHOW_AT_BOTTOM.toString(),
      selected: t === se.SHOW_AT_BOTTOM.toString()
    },
    {
      name: this.localize.term("formSettings_multiPageFormsNavigationShowOnTopAndBottom"),
      value: se.SHOW_AT_TOP.toString() + ", " + se.SHOW_AT_BOTTOM.toString(),
      selected: t === se.SHOW_AT_TOP.toString() + ", " + se.SHOW_AT_BOTTOM.toString()
    }
  ];
};
ud = SO([
  h(EO)
], ud);
var TO = Object.defineProperty, $O = Object.getOwnPropertyDescriptor, sr = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? $O(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && TO(e, o, i), i;
}, pu = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Xt = (t, e, o) => (pu(t, e, "read from private field"), o ? o.call(t) : e.get(t)), dt = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, os = (t, e, o, r) => (pu(t, e, "write to private field"), e.set(t, o), o), Tr = (t, e, o) => (pu(t, e, "access private method"), o), gi, _t, hu, Al, _f, fu, vf, Rl, Sf, Il, wf;
const CO = "form-security-table";
let Rt = class extends st(It) {
  constructor() {
    super(...arguments), dt(this, Al), dt(this, fu), dt(this, Rl), dt(this, Il), this.records = [], dt(this, gi, "formName"), dt(this, _t, "asc"), dt(this, hu, [
      {
        key: "formName",
        label: "formSecurity_formName"
      },
      {
        key: "formCreated",
        label: "content_createDate",
        labelSuffix: "(UTC)"
      },
      {
        key: "hasAccess",
        label: "formSecurity_hasAccess"
      }
    ]);
  }
  get _allFormsAccessible() {
    return this.records.every((t) => t.hasAccess);
  }
  render() {
    return n`<div id="header">
        ${this.localize.term("formSecurity_selectAndDeselect")}
        <uui-toggle
          ?checked=${this._allFormsAccessible}
          @change=${Tr(this, Al, _f)}
        ></uui-toggle>
      </div>
      <uui-table>
        <uui-table-head> ${Tr(this, Il, wf).call(this)} </uui-table-head>
        ${this.records.map(
      (t) => n`<uui-table-row>
              <uui-table-cell>
                <div>${t.formName}</div>
                <small>${t.fields}</small>
              </uui-table-cell>
              <uui-table-cell>
                <umb-localize-date
                  .date=${t.formCreated}
                  .options=${p_}
                ></umb-localize-date>
              </uui-table-cell>
              <uui-table-cell>
                <uui-toggle
                  ?checked=${t.hasAccess}
                  label=${this.localize.term("formSecurity_hasAccess")}
                  @change=${() => Tr(this, Rl, Sf).call(this, t.form)}
                ></uui-toggle>
              </uui-table-cell>
            </uui-table-row>`
    )}
      </uui-table>`;
  }
};
gi = /* @__PURE__ */ new WeakMap();
_t = /* @__PURE__ */ new WeakMap();
hu = /* @__PURE__ */ new WeakMap();
Al = /* @__PURE__ */ new WeakSet();
_f = function() {
  const t = !this._allFormsAccessible;
  this.records.forEach((e) => {
    var o;
    (o = this.set) == null || o.call(this, e.form, t), e = { ...e, hasAccess: t };
  }), this.dispatchEvent(new CustomEvent("valueChange"));
};
fu = /* @__PURE__ */ new WeakSet();
vf = function(t) {
  Xt(this, gi) !== t ? os(this, _t, "asc") : os(this, _t, Xt(this, _t) === "asc" ? "desc" : "asc"), os(this, gi, t), this.records = [...this.records].sort((e, o) => {
    switch (t) {
      case "hasAccess":
        return Number(e.hasAccess) - Number(o.hasAccess);
      case "formCreated":
        return Date.parse(e.formCreated) - Date.parse(o.formCreated);
      default:
        return e[t].localeCompare(o[t]);
    }
  }), Xt(this, _t) === "desc" && this.records.reverse();
};
Rl = /* @__PURE__ */ new WeakSet();
Sf = function(t) {
  var o;
  (o = this.toggle) == null || o.call(this, t);
  let e = this.records.find((r) => r.form === t);
  e && (e = { ...e, hasAccess: !(e != null && e.hasAccess) }), this.dispatchEvent(new CustomEvent("valueChange"));
};
Il = /* @__PURE__ */ new WeakSet();
wf = function() {
  return n`${Xt(this, hu).map(
    (t) => n`<uui-table-head-cell>
        <button @click=${() => Tr(this, fu, vf).call(this, t.key)}>
          ${this.localize.term(t.label) + (t.labelSuffix ? ` ${t.labelSuffix}` : "")}
          <uui-symbol-sort
            ?active=${Xt(this, gi) === t.key}
            ?descending=${Xt(this, _t) === "desc"}
          ></uui-symbol-sort>
        </button>
      </uui-table-head-cell>`
  )}`;
};
Rt.styles = C`
    #header {
      display: flex;
      align-items: center;
      justify-content: flex-end;
      column-gap: var(--uui-size-3);
    }

    uui-table-head-cell button {
      background-color: transparent;
      color: inherit;
      border: none;
      cursor: pointer;
      font-weight: inherit;
      font-size: inherit;
      display: inline-flex;
      align-items: center;
      justify-content: space-between;
      width: 100%;
      padding: var(--uui-size-5) var(--uui-size-1);
    }
  `;
sr([
  p()
], Rt.prototype, "set", 2);
sr([
  p()
], Rt.prototype, "toggle", 2);
sr([
  p({ type: Array })
], Rt.prototype, "records", 2);
sr([
  w()
], Rt.prototype, "_allFormsAccessible", 1);
Rt = sr([
  h(CO)
], Rt);
var OO = Object.defineProperty, PO = Object.getOwnPropertyDescriptor, ja = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? PO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && OO(e, o, i), i;
}, yu = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Wo = (t, e, o) => (yu(t, e, "read from private field"), o ? o.call(t) : e.get(t)), dd = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, kO = (t, e, o, r) => (yu(t, e, "write to private field"), e.set(t, o), o), md = (t, e, o) => (yu(t, e, "access private method"), o), Ne, $r, Ul;
let So = class extends be {
  constructor() {
    super(), dd(this, $r), dd(this, Ne, void 0), this.consumeContext(Da, (t) => {
      var e;
      kO(this, Ne, t), this._startDate = (e = Wo(this, Ne)) == null ? void 0 : e.oneMonthAgo(), this._endDate = Wo(this, Ne).today();
    });
  }
  render() {
    var t, e;
    return n`
      <div class="input-container">
        <uui-label for="start-date">From:</uui-label>
        <uui-input
          id="start-date"
          type="date"
          label="From"
          .max=${(t = Wo(this, Ne)) == null ? void 0 : t.today()}
          .value=${this._startDate}
          @change=${md(this, $r, Ul)}
        ></uui-input>
      </div>
      <div class="input-container">
        <uui-label for="end-date">To: </uui-label>
        <uui-input
          id="end-date"
          type="date"
          label="To"
          .min=${this._startDate}
          .max=${(e = Wo(this, Ne)) == null ? void 0 : e.today()}
          .value=${this._endDate}
          @change=${md(this, $r, Ul)}
        ></uui-input>
      </div>
    `;
  }
};
Ne = /* @__PURE__ */ new WeakMap();
$r = /* @__PURE__ */ new WeakSet();
Ul = function() {
  var t;
  this._inputs.forEach((e) => {
    e.id === "start-date" ? this._startDate = e.value.toString() : e.id === "end-date" && (this._endDate = e.value.toString());
  }), (t = Wo(this, Ne)) == null || t.setFilter({
    startDate: this._startDate,
    endDate: this._endDate
  });
};
So.styles = [
  fa,
  C`
      :host {
        display: flex;
        gap: var(--uui-size-space-5);
      }

      .input-container {
        display: flex;
        align-items: baseline;
        column-gap: var(--uui-size-space-3);
      }
    `
];
ja([
  w()
], So.prototype, "_startDate", 2);
ja([
  w()
], So.prototype, "_endDate", 2);
ja([
  wd("uui-input")
], So.prototype, "_inputs", 2);
So = ja([
  h("form-entry-filter-date-range-selector")
], So);
var DO = Object.defineProperty, xO = Object.getOwnPropertyDescriptor, bf = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? xO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && DO(e, o, i), i;
}, gu = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, MO = (t, e, o) => (gu(t, e, "read from private field"), o ? o.call(t) : e.get(t)), pd = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, AO = (t, e, o, r) => (gu(t, e, "write to private field"), e.set(t, o), o), RO = (t, e, o) => (gu(t, e, "access private method"), o), oa, Wl, Ff;
let ia = class extends be {
  constructor() {
    super(), pd(this, Wl), this._filterText = "", pd(this, oa, void 0), this.consumeContext(Da, (t) => {
      AO(this, oa, t);
    });
  }
  render() {
    return n`
      <uui-input
        label="Filter entries"
        placeholder=${this.localize.term("formEntries_filterEntries")}
        .value=${this._filterText}
        @change=${RO(this, Wl, Ff)}
      ></uui-input>
    `;
  }
};
oa = /* @__PURE__ */ new WeakMap();
Wl = /* @__PURE__ */ new WeakSet();
Ff = function(t) {
  var o;
  const e = t.target.value;
  this._filterText = e.toString(), (o = MO(this, oa)) == null || o.setFilter({ filter: this._filterText });
};
ia.styles = [
  fa,
  C`
      :host,
      uui-input {
        width: 100%;
      }
    `
];
bf([
  w()
], ia.prototype, "_filterText", 2);
ia = bf([
  h("form-entry-filter-text")
], ia);
var IO = Object.defineProperty, UO = Object.getOwnPropertyDescriptor, Ga = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? UO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && IO(e, o, i), i;
}, WO = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, hd = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, Ll = (t, e, o) => (WO(t, e, "access private method"), o), _u, Ef, Cr, zl;
const LO = "ref-form";
let wo = class extends st(
  ay
) {
  constructor() {
    super(...arguments), hd(this, _u), hd(this, Cr), this.selectable = !1, this._count = 0;
  }
  async connectedCallback() {
    var e, o;
    if (super.connectedCallback(), !this.model || !((e = this.config) != null && e.viewEntries))
      return;
    const { count: t } = await Ze.getFormByFormIdRecordMetadata({
      formId: (o = this.model) == null ? void 0 : o.id
    });
    this._count = t;
  }
  render() {
    var t, e, o;
    return n`
      <div id="open-part" tabindex="0">
        <div id="content">
          <span id="icon"><uui-icon .svg=${this.fallbackIcon}></uui-icon></span>
          <div id="info">
            <div id="name">${this.model.name}</div>
            ${this.renderDetail()}
          </div>
        </div>
      </div>
      <div id="select-border"></div>

      <slot name="actions" id="actions-container">
        <uui-action-bar>
          ${m(
      (t = this.config) == null ? void 0 : t.manageForms,
      () => Ll(this, Cr, zl).call(this, "edit", "formsDashboard_editForm")
    )}
          ${m(
      (e = this.config) == null ? void 0 : e.viewEntries,
      () => Ll(this, Cr, zl).call(this, "view", "formsDashboard_viewEntries")
    )}
        </uui-action-bar>
      </slot>
      ${m(
      (o = this.config) == null ? void 0 : o.viewEntries,
      () => n` <uui-tag size="s" slot="tag" color="positive"
          >${this.localize.term(
        "formsDashboard_entriesCount",
        this._count
      )}</uui-tag
        >`
    )}
    `;
  }
};
_u = /* @__PURE__ */ new WeakSet();
Ef = function(t) {
  this.dispatchEvent(new CustomEvent(t));
};
Cr = /* @__PURE__ */ new WeakSet();
zl = function(t, e) {
  return n`<uui-button
      @click=${() => Ll(this, _u, Ef).call(this, t)}
      label=${this.localize.term(e)}
    ></uui-button>`;
};
wo.styles = [
  ...sy.styles,
  C`
      #open-part:hover #name,
      #open-part:hover #icon {
        color: var(--uui-color-text) !important;
        text-decoration: none !important;
      }

      #info {
        min-width: 150px;
      }

      #actions-container {
        margin-left: auto;
      }

      uui-action-bar {
        justify-content: end;
      }

      uui-tag {
        margin: 0 var(--uui-size-8);
      }
    `
];
Ga([
  p({ type: Object })
], wo.prototype, "model", 2);
Ga([
  p({ type: Object })
], wo.prototype, "config", 2);
Ga([
  w()
], wo.prototype, "_count", 2);
wo = Ga([
  h(LO)
], wo);
class zO extends Od {
  constructor(e) {
    super(e, Np, Rg, (o) => o.unique);
  }
}
var NO = Object.defineProperty, qO = Object.getOwnPropertyDescriptor, Lt = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? qO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && NO(e, o, i), i;
}, Tf = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, Qt = (t, e, o) => (Tf(t, e, "read from private field"), o ? o.call(t) : e.get(t)), Le = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, bo = (t, e, o) => (Tf(t, e, "access private method"), o), vt, vu, $f, Su, Cf, wu, bu, Of, Nl, Pf, ql, kf, Fu, Df;
const VO = "forms-input-form";
let Me = class extends Cd(be, "") {
  constructor() {
    super(), Le(this, vu), Le(this, Su), Le(this, bu), Le(this, Nl), Le(this, ql), Le(this, Fu), this.multiple = !1, this.allowedFolders = [], this.allowedForms = [], Le(this, vt, new zO(this)), Le(this, wu, (t) => !t.isFolder && (this.allowedForms.length === 0 || this.allowedForms.indexOf(t.unique) > -1) && (this.allowedFolders.length === 0 || this.allowedFolders.some((e) => Ed(t.path).includes(e)))), this.observe(Qt(this, vt).selectedItems, (t) => this._items = t);
  }
  get selection() {
    return Qt(this, vt).getSelection();
  }
  set selection(t) {
    Qt(this, vt).setSelection(t ?? []);
  }
  async connectedCallback() {
    super.connectedCallback();
    const t = await ie.getSecurityUserCurrentFormSecurity({
      includeFormFieldDetails: !1
    });
    this._userSecurity = t.userSecurity;
  }
  getFormElement() {
  }
  render() {
    return n`${bo(this, Nl, Pf).call(this)} ${bo(this, ql, kf).call(this)}`;
  }
};
vt = /* @__PURE__ */ new WeakMap();
vu = /* @__PURE__ */ new WeakSet();
$f = function(t) {
  history.pushState(
    {},
    "",
    `/umbraco/section/forms/workspace/form/edit/${t}`
  );
};
Su = /* @__PURE__ */ new WeakSet();
Cf = function(t) {
  history.pushState(
    {},
    "",
    `/umbraco/section/forms/workspace/form/edit/${t}/view/entries`
  );
};
wu = /* @__PURE__ */ new WeakMap();
bu = /* @__PURE__ */ new WeakSet();
Of = function() {
  Qt(this, vt).openPicker({
    hideTreeRoot: !0,
    multiple: this.multiple,
    pickableFilter: Qt(this, wu)
  });
};
Nl = /* @__PURE__ */ new WeakSet();
Pf = function() {
  var t;
  if ((t = this._items) != null && t.length)
    return n`<uui-ref-list>
			${Bi(
      this._items,
      (e) => e.unique,
      (e) => bo(this, Fu, Df).call(this, e)
    )}
		</uui-ref-list>`;
};
ql = /* @__PURE__ */ new WeakSet();
kf = function() {
  return n`<uui-button
			id="add-button"
			look="placeholder"
			@click=${bo(this, bu, Of)}
			label=${this.localize.term("general_choose")}></uui-button>`;
};
Fu = /* @__PURE__ */ new WeakSet();
Df = function(t) {
  var e, o;
  if (t.unique)
    return n`
			<uui-ref-node name=${t.name}>
				<umb-icon slot="icon" name="icon-umb-contour"></umb-icon>
				<uui-action-bar slot="actions">
					${m(
      (e = this._userSecurity) == null ? void 0 : e.manageForms,
      () => n`<uui-button
							@click=${() => bo(this, vu, $f).call(this, t.unique)}
							label=${this.localize.term("general_edit")}></uui-button>`
    )}
					${m(
      (o = this._userSecurity) == null ? void 0 : o.viewEntries,
      () => n`<uui-button
							@click=${() => bo(this, Su, Cf).call(this, t.unique)}
							label=${this.localize.term("general_open")}></uui-button>`
    )}
					<uui-button
						@click=${() => Qt(this, vt).requestRemoveItem(t.unique)}
						label=${this.localize.term("formPicker_removeItemButtonLabel", t.name)}>
						${this.localize.term("general_remove")}
					</uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
};
Me.styles = [
  C`
			#add-button {
				width: 100%;
			}

			uui-ref-node[drag-placeholder] {
				opacity: 0.2;
			}
		`
];
Lt([
  p({ type: Array })
], Me.prototype, "selection", 1);
Lt([
  p({ type: Boolean })
], Me.prototype, "multiple", 2);
Lt([
  p({ type: Array })
], Me.prototype, "allowedFolders", 2);
Lt([
  p({ type: Array })
], Me.prototype, "allowedForms", 2);
Lt([
  w()
], Me.prototype, "_userSecurity", 2);
Lt([
  w()
], Me.prototype, "_items", 2);
Me = Lt([
  h(VO)
], Me);
class BO extends Od {
  constructor(e) {
    super(e, qp, Cg, (o) => o.unique);
  }
}
var jO = Object.defineProperty, GO = Object.getOwnPropertyDescriptor, Eu = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? GO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && jO(e, o, i), i;
}, xf = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, St = (t, e, o) => (xf(t, e, "read from private field"), o ? o.call(t) : e.get(t)), Nt = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, ra = (t, e, o) => (xf(t, e, "access private method"), o), qe, Tu, $u, Mf, Vl, Af, Bl, Rf, Cu, If;
const HO = "forms-input-folder";
let _i = class extends Cd(be, "") {
  constructor() {
    super(), Nt(this, $u), Nt(this, Vl), Nt(this, Bl), Nt(this, Cu), Nt(this, qe, new BO(this)), Nt(this, Tu, (t) => t.isFolder), this.observe(St(this, qe).selection, (t) => this.value = t.join(",")), this.observe(St(this, qe).selectedItems, (t) => this._items = t);
  }
  get selection() {
    return St(this, qe).getSelection();
  }
  set selection(t) {
    St(this, qe).setSelection(t ?? []);
  }
  set value(t) {
    this.selection = Ed(t);
  }
  get value() {
    return this.selection.join(",");
  }
  connectedCallback() {
    super.connectedCallback();
  }
  getFormElement() {
  }
  render() {
    return n`${ra(this, Vl, Af).call(this)} ${ra(this, Bl, Rf).call(this)}`;
  }
};
qe = /* @__PURE__ */ new WeakMap();
Tu = /* @__PURE__ */ new WeakMap();
$u = /* @__PURE__ */ new WeakSet();
Mf = function() {
  St(this, qe).openPicker({
    pickableFilter: St(this, Tu)
  });
};
Vl = /* @__PURE__ */ new WeakSet();
Af = function() {
  var t;
  if ((t = this._items) != null && t.length)
    return n`<uui-ref-list>
			${Bi(
      this._items,
      (e) => e.unique,
      (e) => ra(this, Cu, If).call(this, e)
    )}
		</uui-ref-list>`;
};
Bl = /* @__PURE__ */ new WeakSet();
Rf = function() {
  return n`<uui-button
			id="add-button"
			look="placeholder"
			@click=${ra(this, $u, Mf)}
			label=${this.localize.term("general_choose")}></uui-button>`;
};
Cu = /* @__PURE__ */ new WeakSet();
If = function(t) {
  if (t.unique)
    return n`
			<uui-ref-node name=${t.name}>
				<umb-icon slot="icon" name="icon-folder"></umb-icon>
				<uui-action-bar slot="actions">
					<uui-button
						@click=${() => St(this, qe).requestRemoveItem(t.unique)}
						label=${this.localize.term("formPicker_removeItemButtonLabel", t.name)}>
						${this.localize.term("general_remove")}
					</uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
};
_i.styles = [
  C`
			#add-button {
				width: 100%;
			}

			uui-ref-node[drag-placeholder] {
				opacity: 0.2;
			}
		`
];
Eu([
  p()
], _i.prototype, "value", 1);
Eu([
  w()
], _i.prototype, "_items", 2);
_i = Eu([
  h(HO)
], _i);
var qi;
class YO {
  constructor(e) {
    f(this, qi, void 0);
    _(this, qi, e);
  }
  async getCollection() {
    const { data: e, error: o } = await y(
      d(this, qi),
      Ey.getTheme()
    );
    if (o)
      return { error: o };
    if (!e)
      return { data: { items: [], total: 0 } };
    const r = e.map((i) => ({
      name: i.name,
      entityType: "forms-theme",
      unique: i.name
    }));
    return { data: { items: r, total: r.length } };
  }
}
qi = new WeakMap();
var Vi;
class KO {
  constructor(e) {
    f(this, Vi, void 0);
    _(this, Vi, new YO(e));
  }
  async requestCollection() {
    return d(this, Vi).getCollection();
  }
  destroy() {
  }
}
Vi = new WeakMap();
var XO = Object.defineProperty, QO = Object.getOwnPropertyDescriptor, Ou = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? QO(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (i = (r ? s(e, o, i) : s(i)) || i);
  return r && i && XO(e, o, i), i;
}, JO = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, fd = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, yd = (t, e, o) => (JO(t, e, "access private method"), o), jl, Uf, Gl, Wf;
const ZO = "forms-input-theme";
let aa = class extends be {
  constructor() {
    super(...arguments), fd(this, jl), fd(this, Gl), this._options = [], this.value = "";
  }
  async connectedCallback() {
    super.connectedCallback(), await yd(this, jl, Uf).call(this);
  }
  render() {
    return n`
      <uui-select
        name="themePicker"
        label="Theme picker"
        @change=${yd(this, Gl, Wf)}
        .options=${this._options.map((t) => ({
      name: t,
      value: t,
      selected: t === this.value
    }))}
      >
      </uui-select>
    `;
  }
};
jl = /* @__PURE__ */ new WeakSet();
Uf = async function() {
  const t = new KO(this), { data: e } = await t.requestCollection();
  this._options = (e == null ? void 0 : e.items.map((o) => o.name)) ?? [];
};
Gl = /* @__PURE__ */ new WeakSet();
Wf = function(t) {
  const e = t.target.value.toString();
  e !== this.value && (this.value = e, this.dispatchEvent(
    new CustomEvent("change", { composed: !0, bubbles: !0 })
  ));
};
Ou([
  w()
], aa.prototype, "_options", 2);
Ou([
  p()
], aa.prototype, "value", 2);
aa = Ou([
  h(ZO)
], aa);
const eP = [
  ...D$,
  ...d$,
  ...$y,
  ...p$,
  ...P$,
  ...x$
], WP = (t, e) => {
  e.registerMany(eP), t.consumeContext(Bf, async (o) => {
    o && (l.TOKEN = () => o.getLatestToken(), l.WITH_CREDENTIALS = !0);
  });
};
export {
  Fy as $,
  ft as A,
  UP as B,
  Xi as C,
  p_ as D,
  MP as E,
  Ql as F,
  Dt as G,
  H$ as H,
  Or as I,
  Yp as J,
  Nr as K,
  xd as L,
  Zp as M,
  qv as N,
  tr as O,
  U_ as P,
  vy as Q,
  Ze as R,
  ie as S,
  Wc as T,
  RP as U,
  Co as V,
  cT as W,
  Lc as X,
  or as Y,
  gT as Z,
  Sy as _,
  Jl as a,
  Xd as a$,
  Wp as a0,
  he as a1,
  Eo as a2,
  tc as a3,
  US as a4,
  Da as a5,
  Ag as a6,
  xa as a7,
  WP as a8,
  hi as a9,
  ea as aA,
  xl as aB,
  ud as aC,
  Rt as aD,
  Tn as aE,
  Gp as aF,
  AF as aG,
  RF as aH,
  So as aI,
  ia as aJ,
  wo as aK,
  Me as aL,
  _i as aM,
  aa as aN,
  Cg as aO,
  Vd as aP,
  Bd as aQ,
  jd as aR,
  Og as aS,
  Gd as aT,
  Pg as aU,
  Hd as aV,
  kg as aW,
  Yd as aX,
  Dg as aY,
  Kd as aZ,
  xg as a_,
  Br as aa,
  dh as ab,
  a$ as ac,
  mh as ad,
  s$ as ae,
  pi as af,
  de as ag,
  di as ah,
  mi as ai,
  jr as aj,
  Yr as ak,
  Kr as al,
  At as am,
  ye as an,
  _o as ao,
  rt as ap,
  vo as aq,
  at as ar,
  re as as,
  rd as at,
  ad as au,
  sd as av,
  nd as aw,
  ld as ax,
  cd as ay,
  Tl as az,
  K as b,
  Mg as b0,
  Qd as b1,
  Jd as b2,
  Zd as b3,
  em as b4,
  tm as b5,
  Rg as b6,
  om as b7,
  Ig as b8,
  kr as b9,
  xr as ba,
  fe as bb,
  jo as bc,
  Go as bd,
  Ho as be,
  xt as bf,
  ue as bg,
  Mt as bh,
  fn as bi,
  uo as bj,
  Jo as bk,
  po as bl,
  Bu as bm,
  Ae as c,
  Qi as d,
  Ju as e,
  Zu as f,
  YS as g,
  IP as h,
  AP as i,
  sv as j,
  $o as k,
  bS as l,
  Zo as m,
  sg as n,
  wv as o,
  Hi as p,
  Ut as q,
  SS as r,
  vS as s,
  _S as t,
  pt as u,
  AS as v,
  kv as w,
  uS as x,
  Fe as y,
  ya as z
};
//# sourceMappingURL=index.js.map
