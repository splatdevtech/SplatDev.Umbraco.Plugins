var f = (r, t, e) => {
  if (!t.has(r))
    throw TypeError("Cannot " + e);
};
var p = (r, t, e) => (f(r, t, "read from private field"), e ? e.call(r) : t.get(r)), n = (r, t, e) => {
  if (t.has(r))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(r) : t.set(r, e);
}, g = (r, t, e, a) => (f(r, t, "write to private field"), a ? a.call(r, e) : t.set(r, e), e);
var i = (r, t, e) => (f(r, t, "access private method"), e);
import { UmbLocalizationController as z } from "@umbraco-cms/backoffice/localization-api";
import { D as A, h as F } from "./index.js";
var s, o, c, h, D, m, d, V, b, S, w;
class R {
  constructor(t, e) {
    n(this, h);
    n(this, m);
    n(this, V);
    n(this, S);
    n(this, s, void 0);
    n(this, o, void 0);
    n(this, c, []);
    g(this, s, t), g(this, o, new z(p(this, s))), g(this, c, e);
  }
  getRenderValue(t, e) {
    switch (e) {
      case "date":
        return i(this, h, D).call(this, t, F);
      case "datetime":
        return i(this, h, D).call(this, t, A);
      case "number":
        return t.value;
      case "file":
        return i(this, m, d).call(this, t);
      default:
        return i(this, V, b).call(this, t);
    }
  }
}
s = new WeakMap(), o = new WeakMap(), c = new WeakMap(), h = new WeakSet(), D = function(t, e) {
  try {
    return t.value && t.value.toString().length > 0 ? p(this, o).date(t.value.toString(), F) : "";
  } catch {
    return t.value;
  }
}, m = new WeakSet(), d = function(t) {
  return t.value ? t.value.toString().split(", ").map((a) => a.replace(/^.*[\\/]/, "")).join(", ") : "";
}, V = new WeakSet(), b = function(t) {
  if (!t.value || t.value.toString().length === 0)
    return "";
  const e = i(this, S, w).call(this, t.fieldId);
  if (e.length === 0)
    return t.value.toString();
  const a = e.find((u) => u.value === t.value);
  if (a)
    return a.caption && a.caption.length > 0 ? a.caption : t.value;
  const P = Array.isArray(t.value) ? t.value : t.value.toString().split(", ");
  let v = "";
  for (let u = 0; u < P.length; u++) {
    u > 0 && (v += ", ");
    const y = P[u].trim(), l = e.find((x) => x.value === y);
    l != null && l.caption && l.caption.length > 0 ? v += l.caption : v += y;
  }
  return v;
}, S = new WeakSet(), w = function(t) {
  const e = p(this, c).find((a) => a.id === t);
  return e ? e.prevalues : [];
};
export {
  R as E
};
//# sourceMappingURL=entry-render-helper.class.js.map
