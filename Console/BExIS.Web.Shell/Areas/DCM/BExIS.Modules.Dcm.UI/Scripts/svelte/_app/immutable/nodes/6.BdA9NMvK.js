import{s as b,h,j as c,i as y,d as v}from"../chunks/scheduler.CAfox-jx.js";import{S as T,i as k,c as u,a as p,m as $,t as s,b as i,d,g as w,e as S}from"../chunks/index.KweEQ9zd.js";import{p as C,h as I}from"../chunks/eslint4b.es.CUHVcEDL.js";import{w as L}from"../chunks/index.B3w0AL5h.js";import{P}from"../chunks/Page.423dBf39.js";import{T as E}from"../chunks/Table.Cjp2sdtc.js";import{g as N}from"../chunks/BaseCaller.DXOARORR.js";function m(n){let t,a;return t=new E({props:{config:n[0]}}),{c(){u(t.$$.fragment)},l(e){p(t.$$.fragment,e)},m(e,o){$(t,e,o),a=!0},p(e,o){const r={};o&1&&(r.config=e[0]),t.$set(r)},i(e){a||(s(t.$$.fragment,e),a=!0)},o(e){i(t.$$.fragment,e),a=!1},d(e){d(t,e)}}}function j(n){let t,a,e=n[0]&&m(n);return{c(){e&&e.c(),t=c()},l(o){e&&e.l(o),t=c()},m(o,r){e&&e.m(o,r),y(o,t,r),a=!0},p(o,r){o[0]?e?(e.p(o,r),r&1&&s(e,1)):(e=m(o),e.c(),s(e,1),e.m(t.parentNode,t)):e&&(w(),i(e,1,1,()=>{e=null}),S())},i(o){a||(s(e),a=!0)},o(o){i(e),a=!1},d(o){o&&v(t),e&&e.d(o)}}}function q(n){let t,a;return t=new P({props:{contentLayoutType:C.full,$$slots:{default:[j]},$$scope:{ctx:n}}}),{c(){u(t.$$.fragment)},l(e){p(t.$$.fragment,e)},m(e,o){$(t,e,o),a=!0},p(e,[o]){const r={};o&65&&(r.$$scope={dirty:o,ctx:e}),t.$set(r)},i(e){a||(s(t.$$.fragment,e),a=!0)},o(e){i(t.$$.fragment,e),a=!1},d(e){d(t,e)}}}function A(n,t,a){let e,o,r,l="";h(),g();async function g(){e=document.getElementById("test"),o=Number(e==null?void 0:e.getAttribute("dataset"));const _=L([]),f=I+"/api/datatable/";l=await N(),a(0,r={id:"serverTable",entityId:o,versionId:-1,data:_,serverSide:!0,URL:f,token:l}),console.log(f,l,o,r)}return n.$$.update=()=>{n.$$.dirty&1},[r]}class H extends T{constructor(t){super(),k(this,t,A,q,b,{})}}export{H as component};
