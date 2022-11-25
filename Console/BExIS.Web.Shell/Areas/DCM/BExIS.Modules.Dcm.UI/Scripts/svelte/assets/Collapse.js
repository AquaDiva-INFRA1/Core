import{S as B,i as G,s as J,P,v as K,b as j,l as h,t as k,w as R,d as S,x as U,f as H,W as D,z as V,H as F,n as W,X as w,Y as p,Z as x,e as $,_ as L,$ as z,a0 as ee,a1 as te,a2 as ne,o as ie,T as le,Q as oe,a3 as se,j as v}from"./Api.js";import{c as ae,b as re}from"./Caller.js";const M=["touchstart","click"],fe=(e,t)=>{let n;if(typeof e=="string"&&typeof window<"u"&&document&&document.createElement){let a=document.querySelectorAll(e);if(a.length||(a=document.querySelectorAll(`#${e}`)),!a.length)throw new Error(`The target '${e}' could not be identified in the dom, tip: check spelling`);M.forEach(f=>{a.forEach(i=>{i.addEventListener(f,t)})}),n=()=>{M.forEach(f=>{a.forEach(i=>{i.removeEventListener(f,t)})})}}return()=>{typeof n=="function"&&(n(),n=void 0)}};function N(e){let t,n,a,f,i,r,u;const c=e[16].default,s=x(c,e,e[15],null);let E=[{style:n=e[2]?void 0:"overflow: hidden;"},e[9],{class:e[8]}],m={};for(let o=0;o<E.length;o+=1)m=W(m,E[o]);return{c(){t=$("div"),s&&s.c(),L(t,m)},m(o,d){j(o,t,d),s&&s.m(t,null),i=!0,r||(u=[h(t,"introstart",e[17]),h(t,"introend",e[18]),h(t,"outrostart",e[19]),h(t,"outroend",e[20]),h(t,"introstart",function(){z(e[3])&&e[3].apply(this,arguments)}),h(t,"introend",function(){z(e[4])&&e[4].apply(this,arguments)}),h(t,"outrostart",function(){z(e[5])&&e[5].apply(this,arguments)}),h(t,"outroend",function(){z(e[6])&&e[6].apply(this,arguments)})],r=!0)},p(o,d){e=o,s&&s.p&&(!i||d&32768)&&ee(s,c,e,e[15],i?ne(c,e[15],d,null):te(e[15]),null),L(t,m=ie(E,[(!i||d&4&&n!==(n=e[2]?void 0:"overflow: hidden;"))&&{style:n},d&512&&e[9],(!i||d&256)&&{class:e[8]}]))},i(o){i||(k(s,o),P(()=>{f&&f.end(1),a=le(t,ae,{horizontal:e[1]}),a.start()}),i=!0)},o(o){S(s,o),a&&a.invalidate(),o&&(f=oe(t,re,{horizontal:e[1]})),i=!1},d(o){o&&H(t),s&&s.d(o),o&&f&&f.end(),r=!1,se(u)}}}function ue(e){let t,n,a,f;P(e[21]);let i=e[0]&&N(e);return{c(){i&&i.c(),t=K()},m(r,u){i&&i.m(r,u),j(r,t,u),n=!0,a||(f=h(window,"resize",e[21]),a=!0)},p(r,[u]){r[0]?i?(i.p(r,u),u&1&&k(i,1)):(i=N(r),i.c(),k(i,1),i.m(t.parentNode,t)):i&&(R(),S(i,1,1,()=>{i=null}),U())},i(r){n||(k(i),n=!0)},o(r){S(i),n=!1},d(r){i&&i.d(r),r&&H(t),a=!1,f()}}}function de(e,t,n){let a;const f=["isOpen","class","horizontal","navbar","onEntering","onEntered","onExiting","onExited","expand","toggler"];let i=D(t,f),{$$slots:r={},$$scope:u}=t;const c=V();let{isOpen:s=!1}=t,{class:E=""}=t,{horizontal:m=!1}=t,{navbar:o=!1}=t,{onEntering:d=()=>c("opening")}=t,{onEntered:q=()=>c("open")}=t,{onExiting:C=()=>c("closing")}=t,{onExited:T=()=>c("close")}=t,{expand:_=!1}=t,{toggler:O=null}=t;F(()=>fe(O,l=>{n(0,s=!s),l.preventDefault()}));let b=0,y=!1;const g={};g.xs=0,g.sm=576,g.md=768,g.lg=992,g.xl=1200;function A(){c("update",s)}function I(l){v.call(this,e,l)}function Q(l){v.call(this,e,l)}function X(l){v.call(this,e,l)}function Y(l){v.call(this,e,l)}function Z(){n(7,b=window.innerWidth)}return e.$$set=l=>{t=W(W({},t),w(l)),n(9,i=D(t,f)),"isOpen"in l&&n(0,s=l.isOpen),"class"in l&&n(10,E=l.class),"horizontal"in l&&n(1,m=l.horizontal),"navbar"in l&&n(2,o=l.navbar),"onEntering"in l&&n(3,d=l.onEntering),"onEntered"in l&&n(4,q=l.onEntered),"onExiting"in l&&n(5,C=l.onExiting),"onExited"in l&&n(6,T=l.onExited),"expand"in l&&n(11,_=l.expand),"toggler"in l&&n(12,O=l.toggler),"$$scope"in l&&n(15,u=l.$$scope)},e.$$.update=()=>{e.$$.dirty&1030&&n(8,a=p(E,{"collapse-horizontal":m,"navbar-collapse":o})),e.$$.dirty&26757&&o&&_&&(b>=g[_]&&!s?(n(0,s=!0),n(13,y=!0),A()):b<g[_]&&y&&(n(0,s=!1),n(13,y=!1),A()))},[s,m,o,d,q,C,T,b,a,i,E,_,O,y,g,u,r,I,Q,X,Y,Z]}class me extends B{constructor(t){super(),G(this,t,de,ue,J,{isOpen:0,class:10,horizontal:1,navbar:2,onEntering:3,onEntered:4,onExiting:5,onExited:6,expand:11,toggler:12})}}export{me as C};
