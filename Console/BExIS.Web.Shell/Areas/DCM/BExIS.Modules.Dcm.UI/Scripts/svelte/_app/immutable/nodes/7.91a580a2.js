import{S as q,i as C,s as E,w as z,V as G,k as b,q as $,a as k,y as H,l as y,m as B,r as v,h as n,c as w,z as L,b as m,G as A,A as M,W as P,u as R,g as V,d as W,B as j,o as D}from"../chunks/index.5cde3559.js";import{s as F,A as I}from"../chunks/store.53a40dd4.js";import{S as J}from"../chunks/SlideToggle.ec9f182f.js";function K(i){let a,c,o,s,l,_,d,u,r,h,g,p;function T(e){i[1](e)}let S={name:"david"};return i[0]!==void 0&&(S.checked=i[0]),s=new J({props:S}),z.push(()=>G(s,"checked",T)),{c(){a=b("h1"),c=$("Test with layout"),o=k(),H(s.$$.fragment),_=k(),d=b("br"),u=k(),r=b("b"),h=$("toggle : "),g=$(i[0])},l(e){a=y(e,"H1",{});var t=B(a);c=v(t,"Test with layout"),t.forEach(n),o=w(e),L(s.$$.fragment,e),_=w(e),d=y(e,"BR",{}),u=w(e),r=y(e,"B",{});var f=B(r);h=v(f,"toggle : "),g=v(f,i[0]),f.forEach(n)},m(e,t){m(e,a,t),A(a,c),m(e,o,t),M(s,e,t),m(e,_,t),m(e,d,t),m(e,u,t),m(e,r,t),A(r,h),A(r,g),p=!0},p(e,[t]){const f={};!l&&t&1&&(l=!0,f.checked=e[0],P(()=>l=!1)),s.$set(f),(!p||t&1)&&R(g,e[0])},i(e){p||(V(s.$$.fragment,e),p=!0)},o(e){W(s.$$.fragment,e),p=!1},d(e){e&&n(a),e&&n(o),j(s,e),e&&n(_),e&&n(d),e&&n(u),e&&n(r)}}}function N(i,a,c){let o;D(async()=>{console.log("start entity template"),F("https://localhost:44345","davidschoene","123456");try{const l=await I.get("/dcm/entitytemplates/Load");console.log("data",l.data)}catch(l){console.error(l)}});function s(l){o=l,c(0,o)}return c(0,o=!1),[o,s]}class X extends q{constructor(a){super(),C(this,a,N,K,E,{})}}export{X as component};
