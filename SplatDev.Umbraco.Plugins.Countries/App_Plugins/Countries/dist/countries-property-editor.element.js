import { customElement as ee, LitElement as te, property as P, state as E, html as k, nothing as ie, css as re } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as ae } from "@umbraco-cms/backoffice/element-api";
import { UmbChangeEvent as se } from "@umbraco-cms/backoffice/event";
import { UMB_AUTH_CONTEXT as oe } from "@umbraco-cms/backoffice/auth";
import { UMB_NOTIFICATION_CONTEXT as ne } from "@umbraco-cms/backoffice/notification";
function le(e) {
  let t = null, i = null;
  const c = e.consumeContext.bind(e), o = new Promise((a) => {
    c(oe, async (l) => {
      var n;
      try {
        t = await ((n = l == null ? void 0 : l.getLatestToken) == null ? void 0 : n.call(l)) ?? null;
      } catch {
        t = null;
      }
      a();
    }), setTimeout(a, 3e3);
  });
  return c(ne, (a) => {
    i = a;
  }), async (a, l = {}) => {
    await o;
    const n = new Headers(l.headers);
    t && !n.has("Authorization") && n.set("Authorization", `Bearer ${t}`);
    const p = await fetch(a, { ...l, credentials: "same-origin", headers: n });
    if (!p.ok) {
      const v = p.status === 401 || p.status === 403, w = v ? "Not authorised" : "Could not load data", r = v ? `The backoffice token was ${t ? "sent but rejected" : "not available"} (${p.status}). Anything shown below may be empty because the request was refused, not because there is nothing to show.` : `The request failed with ${p.status}. Anything shown below may be incomplete.`;
      console.error(`[SplatDev] ${p.status} from ${String(a)} — ${r}`), i == null || i.peek("danger", { data: { headline: w, message: r } });
    }
    return p;
  };
}
var ce = Object.create, I = Object.defineProperty, ue = Object.getOwnPropertyDescriptor, q = (e, t) => (t = Symbol[e]) ? t : Symbol.for("Symbol." + e), g = (e) => {
  throw TypeError(e);
}, de = (e, t, i) => t in e ? I(e, t, { enumerable: !0, configurable: !0, writable: !0, value: i }) : e[t] = i, F = (e, t) => I(e, "name", { value: t, configurable: !0 }), he = (e) => [, , , ce((e == null ? void 0 : e[q("metadata")]) ?? null)], G = ["class", "method", "getter", "setter", "accessor", "field", "value", "get", "set"], C = (e) => e !== void 0 && typeof e != "function" ? g("Function expected") : e, _e = (e, t, i, c, o) => ({ kind: G[e], name: t, metadata: c, addInitializer: (a) => i._ ? g("Already initialized") : o.push(C(a || null)) }), pe = (e, t) => de(t, q("metadata"), e[3]), u = (e, t, i, c) => {
  for (var o = 0, a = e[t >> 1], l = a && a.length; o < l; o++) t & 1 ? a[o].call(i) : c = a[o].call(i, c);
  return c;
}, m = (e, t, i, c, o, a) => {
  var l, n, p, v, w, r = t & 7, $ = !!(t & 8), f = !!(t & 16), S = r > 3 ? e.length + 1 : r ? $ ? 1 : 2 : 0, B = G[r + 5], U = r > 3 && (e[S - 1] = []), j = e[S] || (e[S] = []), _ = r && (!f && !$ && (o = o.prototype), r < 5 && (r > 3 || !f) && ue(r < 4 ? o : { get [i]() {
    return A(this, a);
  }, set [i](d) {
    return W(this, a, d);
  } }, i));
  r ? f && r < 4 && F(a, (r > 2 ? "set " : r > 1 ? "get " : "") + i) : F(o, i);
  for (var T = c.length - 1; T >= 0; T--)
    v = _e(r, i, p = {}, e[3], j), r && (v.static = $, v.private = f, w = v.access = { has: f ? (d) => fe(o, d) : (d) => i in d }, r ^ 3 && (w.get = f ? (d) => (r ^ 1 ? A : b)(d, o, r ^ 4 ? a : _.get) : (d) => d[i]), r > 2 && (w.set = f ? (d, O) => W(d, o, O, r ^ 4 ? a : _.set) : (d, O) => d[i] = O)), n = (0, c[T])(r ? r < 4 ? f ? a : _[B] : r > 4 ? void 0 : { get: _.get, set: _.set } : o, v), p._ = 1, r ^ 4 || n === void 0 ? C(n) && (r > 4 ? U.unshift(n) : r ? f ? a = n : _[B] = n : o = n) : typeof n != "object" || n === null ? g("Object expected") : (C(l = n.get) && (_.get = l), C(l = n.set) && (_.set = l), C(l = n.init) && U.unshift(l));
  return r || pe(e, o), _ && I(o, i, _), f ? r ^ 4 ? a : _ : o;
}, M = (e, t, i) => t.has(e) || g("Cannot " + i), fe = (e, t) => Object(t) !== t ? g('Cannot use the "in" operator on this value') : e.has(t), A = (e, t, i) => (M(e, t, "read from private field"), i ? i.call(e) : t.get(e)), D = (e, t, i) => t.has(e) ? g("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), W = (e, t, i, c) => (M(e, t, "write to private field"), c ? c.call(e, i) : t.set(e, i), i), b = (e, t, i) => (M(e, t, "access private method"), i), H, L, V, X, J, K, Q, z, R, s, N, y, Y, x, Z;
R = [ee("countries-property-editor")];
class h extends (z = ae(te), Q = [P({ type: String })], K = [P({ type: Boolean })], J = [E()], X = [E()], V = [E()], L = [E()], H = [P({ attribute: !1 })], z) {
  constructor() {
    super(...arguments), u(s, 5, this), D(this, y), this.value = u(s, 8, this, ""), u(s, 11, this), this.readonly = u(s, 12, this, !1), u(s, 15, this), D(this, N, le(this)), this._countries = u(s, 16, this, []), u(s, 19, this), this._loaded = u(s, 20, this, !1), u(s, 23, this), this._failed = u(s, 24, this, !1), u(s, 27, this), this._store = u(s, 28, this, "alpha2"), u(s, 31, this);
  }
  set config(t) {
    if (!t) return;
    const i = t.getValueByAlias("storeValue");
    (i === "alpha3" || i === "name" || i === "numeric" || i === "alpha2") && (this._store = i);
  }
  connectedCallback() {
    super.connectedCallback(), b(this, y, Y).call(this);
  }
  render() {
    return this._loaded ? this._failed || this._countries.length === 0 ? k`
        <div class="warn">
          The country list is empty. It is filled by this plugin's migration on start-up —
          if it stayed empty, check the log for the countries migration.
        </div>
        ${this.value ? k`<p class="hint">Currently holding <code>${this.value}</code>.</p>` : ie}
      ` : k`
      <uui-select
        label="Country"
        ?disabled=${this.readonly}
        .value=${this.value ?? ""}
        @change=${b(this, y, Z)}
        .options=${[
      { name: "— none —", value: "", selected: !this.value },
      ...this._countries.map((t) => ({
        name: t.name,
        value: b(this, y, x).call(this, t),
        selected: b(this, y, x).call(this, t) === this.value
      }))
    ]}
      ></uui-select>
      <p class="hint">
        Stores the ${this._store === "name" ? "country name" : this._store === "numeric" ? "numeric code" : `${this._store === "alpha3" ? "three" : "two"}-letter code`}.
      </p>
    ` : k`<uui-loader></uui-loader>`;
  }
}
s = he(z);
N = /* @__PURE__ */ new WeakMap();
y = /* @__PURE__ */ new WeakSet();
Y = async function() {
  try {
    const e = await A(this, N).call(this, "/umbraco/api/countries/GetCountries");
    e.ok ? this._countries = await e.json() : this._failed = !0;
  } catch {
    this._failed = !0;
  } finally {
    this._loaded = !0;
  }
};
x = function(e) {
  switch (this._store) {
    case "alpha3":
      return e.alpha3Code;
    case "name":
      return e.name;
    case "numeric":
      return String(e.numCode);
    default:
      return e.alpha2Code;
  }
};
Z = function(e) {
  const t = e.target.value;
  t !== this.value && (this.value = t, this.dispatchEvent(new se()));
};
m(s, 3, "config", H, h);
m(s, 5, "value", Q, h);
m(s, 5, "readonly", K, h);
m(s, 5, "_countries", J, h);
m(s, 5, "_loaded", X, h);
m(s, 5, "_failed", V, h);
m(s, 5, "_store", L, h);
h = m(s, 0, "CountriesPropertyEditorElement", R, h);
h.styles = re`
    :host { display: block; }
    uui-select { width: 100%; max-width: 420px; }
    .hint { margin: 6px 0 0; font-size: 0.83rem; color: var(--uui-color-text-alt, #6b7280); }
    .warn {
      margin: 6px 0 0; padding: 8px 10px; border-radius: 3px; font-size: 0.85rem;
      border-left: 3px solid var(--uui-color-warning, #d8a012);
      background: var(--uui-color-warning-emphasis, #fdf6e3);
    }
    code {
      font-family: var(--uui-font-monospace, monospace);
      background: var(--uui-color-surface-alt, #f3f4f6); padding: 1px 5px; border-radius: 3px;
    }
  `;
u(s, 1, h);
const Ce = h;
export {
  h as CountriesPropertyEditorElement,
  Ce as default
};
