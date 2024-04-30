function __vite__mapDeps(indexes) {
  if (!__vite__mapDeps.viteFileDeps) {
    __vite__mapDeps.viteFileDeps = ["../nodes/0.DN6Vjid8.js","../chunks/scheduler.C_o3LU93.js","../chunks/index.CiHPK1yE.js","../chunks/stores.B4wH1ktA.js","../chunks/index.CH-8sbRF.js","../chunks/stores.B4f2X8Az.js","../chunks/stores.B7mFRYPc.js","../chunks/ProgressBar.svelte_svelte_type_style_lang.BJ9KW1aQ.js","../assets/ProgressBar.Cirlo5Z8.css","../assets/0.C9yfffof.css","../nodes/1.D4IJcBhE.js","../chunks/entry.DBG65q7V.js","../nodes/2.BQQZoyTc.js","../chunks/eslint4b.es.DddlA3jT.js","../assets/eslint4b.bRh5xSvE.css","../chunks/Page.Bkl3-QFW.js","../nodes/3.C5xq1BKn.js","../chunks/Spinner.BSaonwBy.js","../chunks/TextInput.h7YpBags.js","../chunks/vest.production.BXOqUVYh.js","../chunks/BaseCaller.ClKx0L_4.js","../nodes/4.lSxwSV2S.js","../chunks/MultiSelect.DyWTFnni.js","../chunks/SlideToggle.DDU2EjEq.js","../chunks/Table.B_UXwKgx.js","../assets/4.BJQo6MCZ.css","../nodes/5.CyGzHVBA.js","../nodes/6.tb_X8p-6.js","../nodes/7.CxD7-UTw.js"]
  }
  return indexes.map((i) => __vite__mapDeps.viteFileDeps[i])
}
import{s as q,j as B,K as h,l as U,i as k,d as g,L as j,M as F,e as G,c as K,k as Q,a as D,N as d,E as W,F as z,G as H,O as I,P as y,Q as J}from"../chunks/scheduler.C_o3LU93.js";import{S as X,i as Y,b as w,e as A,t as b,g as O,c as P,a as T,m as R,d as L}from"../chunks/index.CiHPK1yE.js";const Z="modulepreload",M=function(a,e){return new URL(a,e).href},V={},p=function(e,n,i){let r=Promise.resolve();if(n&&n.length>0){const _=document.getElementsByTagName("link"),t=document.querySelector("meta[property=csp-nonce]"),s=(t==null?void 0:t.nonce)||(t==null?void 0:t.getAttribute("nonce"));r=Promise.all(n.map(o=>{if(o=M(o,i),o in V)return;V[o]=!0;const f=o.endsWith(".css"),l=f?'[rel="stylesheet"]':"";if(!!i)for(let E=_.length-1;E>=0;E--){const v=_[E];if(v.href===o&&(!f||v.rel==="stylesheet"))return}else if(document.querySelector(`link[href="${o}"]${l}`))return;const u=document.createElement("link");if(u.rel=f?"stylesheet":Z,f||(u.as="script",u.crossOrigin=""),u.href=o,s&&u.setAttribute("nonce",s),document.head.appendChild(u),f)return new Promise((E,v)=>{u.addEventListener("load",E),u.addEventListener("error",()=>v(new Error(`Unable to preload CSS for ${o}`)))})}))}return r.then(()=>e()).catch(_=>{const t=new Event("vite:preloadError",{cancelable:!0});if(t.payload=_,window.dispatchEvent(t),!t.defaultPrevented)throw _})},se={};function $(a){let e,n,i;var r=a[1][0];function _(t,s){return{props:{data:t[3],form:t[2]}}}return r&&(e=y(r,_(a)),a[12](e)),{c(){e&&P(e.$$.fragment),n=h()},l(t){e&&T(e.$$.fragment,t),n=h()},m(t,s){e&&R(e,t,s),k(t,n,s),i=!0},p(t,s){if(s&2&&r!==(r=t[1][0])){if(e){O();const o=e;w(o.$$.fragment,1,0,()=>{L(o,1)}),A()}r?(e=y(r,_(t)),t[12](e),P(e.$$.fragment),b(e.$$.fragment,1),R(e,n.parentNode,n)):e=null}else if(r){const o={};s&8&&(o.data=t[3]),s&4&&(o.form=t[2]),e.$set(o)}},i(t){i||(e&&b(e.$$.fragment,t),i=!0)},o(t){e&&w(e.$$.fragment,t),i=!1},d(t){t&&g(n),a[12](null),e&&L(e,t)}}}function x(a){let e,n,i;var r=a[1][0];function _(t,s){return{props:{data:t[3],$$slots:{default:[ee]},$$scope:{ctx:t}}}}return r&&(e=y(r,_(a)),a[11](e)),{c(){e&&P(e.$$.fragment),n=h()},l(t){e&&T(e.$$.fragment,t),n=h()},m(t,s){e&&R(e,t,s),k(t,n,s),i=!0},p(t,s){if(s&2&&r!==(r=t[1][0])){if(e){O();const o=e;w(o.$$.fragment,1,0,()=>{L(o,1)}),A()}r?(e=y(r,_(t)),t[11](e),P(e.$$.fragment),b(e.$$.fragment,1),R(e,n.parentNode,n)):e=null}else if(r){const o={};s&8&&(o.data=t[3]),s&8215&&(o.$$scope={dirty:s,ctx:t}),e.$set(o)}},i(t){i||(e&&b(e.$$.fragment,t),i=!0)},o(t){e&&w(e.$$.fragment,t),i=!1},d(t){t&&g(n),a[11](null),e&&L(e,t)}}}function ee(a){let e,n,i;var r=a[1][1];function _(t,s){return{props:{data:t[4],form:t[2]}}}return r&&(e=y(r,_(a)),a[10](e)),{c(){e&&P(e.$$.fragment),n=h()},l(t){e&&T(e.$$.fragment,t),n=h()},m(t,s){e&&R(e,t,s),k(t,n,s),i=!0},p(t,s){if(s&2&&r!==(r=t[1][1])){if(e){O();const o=e;w(o.$$.fragment,1,0,()=>{L(o,1)}),A()}r?(e=y(r,_(t)),t[10](e),P(e.$$.fragment),b(e.$$.fragment,1),R(e,n.parentNode,n)):e=null}else if(r){const o={};s&16&&(o.data=t[4]),s&4&&(o.form=t[2]),e.$set(o)}},i(t){i||(e&&b(e.$$.fragment,t),i=!0)},o(t){e&&w(e.$$.fragment,t),i=!1},d(t){t&&g(n),a[10](null),e&&L(e,t)}}}function S(a){let e,n=a[6]&&N(a);return{c(){e=G("div"),n&&n.c(),this.h()},l(i){e=K(i,"DIV",{id:!0,"aria-live":!0,"aria-atomic":!0,style:!0});var r=Q(e);n&&n.l(r),r.forEach(g),this.h()},h(){D(e,"id","svelte-announcer"),D(e,"aria-live","assertive"),D(e,"aria-atomic","true"),d(e,"position","absolute"),d(e,"left","0"),d(e,"top","0"),d(e,"clip","rect(0 0 0 0)"),d(e,"clip-path","inset(50%)"),d(e,"overflow","hidden"),d(e,"white-space","nowrap"),d(e,"width","1px"),d(e,"height","1px")},m(i,r){k(i,e,r),n&&n.m(e,null)},p(i,r){i[6]?n?n.p(i,r):(n=N(i),n.c(),n.m(e,null)):n&&(n.d(1),n=null)},d(i){i&&g(e),n&&n.d()}}}function N(a){let e;return{c(){e=W(a[7])},l(n){e=z(n,a[7])},m(n,i){k(n,e,i)},p(n,i){i&128&&H(e,n[7])},d(n){n&&g(e)}}}function te(a){let e,n,i,r,_;const t=[x,$],s=[];function o(l,m){return l[1][1]?0:1}e=o(a),n=s[e]=t[e](a);let f=a[5]&&S(a);return{c(){n.c(),i=B(),f&&f.c(),r=h()},l(l){n.l(l),i=U(l),f&&f.l(l),r=h()},m(l,m){s[e].m(l,m),k(l,i,m),f&&f.m(l,m),k(l,r,m),_=!0},p(l,[m]){let u=e;e=o(l),e===u?s[e].p(l,m):(O(),w(s[u],1,1,()=>{s[u]=null}),A(),n=s[e],n?n.p(l,m):(n=s[e]=t[e](l),n.c()),b(n,1),n.m(i.parentNode,i)),l[5]?f?f.p(l,m):(f=S(l),f.c(),f.m(r.parentNode,r)):f&&(f.d(1),f=null)},i(l){_||(b(n),_=!0)},o(l){w(n),_=!1},d(l){l&&(g(i),g(r)),s[e].d(l),f&&f.d(l)}}}function ne(a,e,n){let{stores:i}=e,{page:r}=e,{constructors:_}=e,{components:t=[]}=e,{form:s}=e,{data_0:o=null}=e,{data_1:f=null}=e;j(i.page.notify);let l=!1,m=!1,u=null;F(()=>{const c=i.page.subscribe(()=>{l&&(n(6,m=!0),J().then(()=>{n(7,u=document.title||"untitled page")}))});return n(5,l=!0),c});function E(c){I[c?"unshift":"push"](()=>{t[1]=c,n(0,t)})}function v(c){I[c?"unshift":"push"](()=>{t[0]=c,n(0,t)})}function C(c){I[c?"unshift":"push"](()=>{t[0]=c,n(0,t)})}return a.$$set=c=>{"stores"in c&&n(8,i=c.stores),"page"in c&&n(9,r=c.page),"constructors"in c&&n(1,_=c.constructors),"components"in c&&n(0,t=c.components),"form"in c&&n(2,s=c.form),"data_0"in c&&n(3,o=c.data_0),"data_1"in c&&n(4,f=c.data_1)},a.$$.update=()=>{a.$$.dirty&768&&i.page.set(r)},[t,_,s,o,f,l,m,u,i,r,E,v,C]}class oe extends X{constructor(e){super(),Y(this,e,ne,te,q,{stores:8,page:9,constructors:1,components:0,form:2,data_0:3,data_1:4})}}const ae=[()=>p(()=>import("../nodes/0.DN6Vjid8.js"),__vite__mapDeps([0,1,2,3,4,5,6,7,8,9]),import.meta.url),()=>p(()=>import("../nodes/1.D4IJcBhE.js"),__vite__mapDeps([10,1,2,11,4]),import.meta.url),()=>p(()=>import("../nodes/2.BQQZoyTc.js"),__vite__mapDeps([12,1,2,13,4,14,7,8,15,5]),import.meta.url),()=>p(()=>import("../nodes/3.C5xq1BKn.js"),__vite__mapDeps([16,2,1,13,4,14,7,8,17,15,5,18,19,20]),import.meta.url),()=>p(()=>import("../nodes/4.lSxwSV2S.js"),__vite__mapDeps([21,2,1,13,4,14,7,8,22,17,15,5,3,18,23,6,20,24,11,25]),import.meta.url),()=>p(()=>import("../nodes/5.CyGzHVBA.js"),__vite__mapDeps([26,2,1,13,4,14,7,8,22,17,15,5,3,18,19,23]),import.meta.url),()=>p(()=>import("../nodes/6.tb_X8p-6.js"),__vite__mapDeps([27,2,1,13,4,14,7,8,15,5,24,17,23,20]),import.meta.url),()=>p(()=>import("../nodes/7.CxD7-UTw.js"),__vite__mapDeps([28,2,1,13,4,14,7,8,17]),import.meta.url)],le=[],fe={"/":[2],"/create":[3],"/edit":[4],"/entitytemplates":[5],"/test":[6],"/view":[7]},ce={handleError:({error:a})=>{console.error(a)},reroute:()=>{}};export{fe as dictionary,ce as hooks,se as matchers,ae as nodes,oe as root,le as server_loads};
