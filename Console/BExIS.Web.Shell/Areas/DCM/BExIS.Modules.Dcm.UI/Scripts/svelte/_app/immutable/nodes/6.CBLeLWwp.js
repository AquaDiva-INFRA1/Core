import{S as b,i as v,c as m,a as p,m as _,t as s,b as i,d as $,g as h,e as y}from"../chunks/index.CiHPK1yE.js";import{h as k}from"../chunks/eslint4b.es.Ngf91g9x.js";import"../chunks/ProgressBar.svelte_svelte_type_style_lang.BJ9KW1aQ.js";import{s as S,K as c,i as T,d as w}from"../chunks/scheduler.C_o3LU93.js";import{w as P}from"../chunks/index.CH-8sbRF.js";import{P as I}from"../chunks/Page.BhUOr-Xw.js";import{T as j}from"../chunks/Table.RVhvsgSk.js";import{g as C}from"../chunks/BaseCaller.DjZVoOtO.js";function N(){return{}}const D=Object.freeze(Object.defineProperty({__proto__:null,load:N},Symbol.toStringTag,{value:"Module"}));function u(a){let t,r;return t=new j({props:{config:a[0]}}),{c(){m(t.$$.fragment)},l(e){p(t.$$.fragment,e)},m(e,o){_(t,e,o),r=!0},p(e,o){const n={};o&1&&(n.config=e[0]),t.$set(n)},i(e){r||(s(t.$$.fragment,e),r=!0)},o(e){i(t.$$.fragment,e),r=!1},d(e){$(t,e)}}}function O(a){let t,r,e=a[0]&&u(a);return{c(){e&&e.c(),t=c()},l(o){e&&e.l(o),t=c()},m(o,n){e&&e.m(o,n),T(o,t,n),r=!0},p(o,n){o[0]?e?(e.p(o,n),n&1&&s(e,1)):(e=u(o),e.c(),s(e,1),e.m(t.parentNode,t)):e&&(h(),i(e,1,1,()=>{e=null}),y())},i(o){r||(s(e),r=!0)},o(o){i(e),r=!1},d(o){o&&w(t),e&&e.d(o)}}}function q(a){let t,r;return t=new I({props:{$$slots:{default:[O]},$$scope:{ctx:a}}}),{c(){m(t.$$.fragment)},l(e){p(t.$$.fragment,e)},m(e,o){_(t,e,o),r=!0},p(e,[o]){const n={};o&33&&(n.$$scope={dirty:o,ctx:e}),t.$set(n)},i(e){r||(s(t.$$.fragment,e),r=!0)},o(e){i(t.$$.fragment,e),r=!1},d(e){$(t,e)}}}function z(a,t,r){let e,o,n,l="";d();async function d(){e=document.getElementById("test"),o=Number(e==null?void 0:e.getAttribute("dataset"));const g=P([]),f=k+"/api/datatable/";l=await C(),r(0,n={id:"serverTable",entityId:o,versionId:-1,data:g,serverSide:!0,URL:f,token:l}),console.log(f,l,o,n)}return a.$$.update=()=>{a.$$.dirty&1},[n]}class F extends b{constructor(t){super(),v(this,t,z,q,S,{})}}export{F as component,D as universal};
