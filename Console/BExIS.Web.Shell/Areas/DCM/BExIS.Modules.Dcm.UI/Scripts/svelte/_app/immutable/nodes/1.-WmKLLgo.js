import{s as S,e as _,F as d,l as q,c as f,m as g,G as h,d as l,o as x,i as m,q as v,H as $,n as E,I as H}from"../chunks/scheduler.CAfox-jx.js";import{S as y,i as C}from"../chunks/index.KweEQ9zd.js";import{s as F}from"../chunks/entry.BXKISWBc.js";const G=()=>{const s=F;return{page:{subscribe:s.page.subscribe},navigating:{subscribe:s.navigating.subscribe},updated:s.updated}},I={subscribe(s){return G().page.subscribe(s)}};function P(s){var b;let t,r=s[0].status+"",o,n,i,c=((b=s[0].error)==null?void 0:b.message)+"",u;return{c(){t=_("h1"),o=d(r),n=q(),i=_("p"),u=d(c)},l(e){t=f(e,"H1",{});var a=g(t);o=h(a,r),a.forEach(l),n=x(e),i=f(e,"P",{});var p=g(i);u=h(p,c),p.forEach(l)},m(e,a){m(e,t,a),v(t,o),m(e,n,a),m(e,i,a),v(i,u)},p(e,[a]){var p;a&1&&r!==(r=e[0].status+"")&&$(o,r),a&1&&c!==(c=((p=e[0].error)==null?void 0:p.message)+"")&&$(u,c)},i:E,o:E,d(e){e&&(l(t),l(n),l(i))}}}function j(s,t,r){let o;return H(s,I,n=>r(0,o=n)),[o]}let A=class extends y{constructor(t){super(),C(this,t,j,P,S,{})}};export{A as component};
