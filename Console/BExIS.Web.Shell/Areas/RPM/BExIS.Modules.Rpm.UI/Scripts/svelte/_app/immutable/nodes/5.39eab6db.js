import{F as pt,q as gl,c as nl,d as wt,h as Ke,f as _l,r as ml,t as al,P as hl,p as bl}from"../chunks/eslint4b.es.291451ed.js";import"../chunks/ProgressBar.svelte_svelte_type_style_lang.0e3888ae.js";import{S as Et,i as Dt,s as yt,w as We,Y as Xe,k as g,y as ee,a as A,l as _,m,z as te,h as i,c as T,n as c,b as ge,H as n,A as le,M as K,_ as Ze,g as C,$ as ol,a5 as $t,d as R,B as re,O as At,W as il,q as F,r as G,I as ct,e as Ue,v as gt,f as _t,o as vl,Z as kl,u as dt,a2 as It,T as Ie,a3 as wl,a4 as $l}from"../chunks/index.59d1c272.js";import{A as El,V as Dl,d as yl}from"../chunks/help.511bc58b.js";import{c as Sl,s as Vl,l as Il,a as Al,e as Tl,b as Nl,d as Ml,f as Cl}from"../chunks/services.9a728a1c.js";import{g as St,S as Tt}from"../chunks/BaseCaller.944e72b4.js";import{D as Vt}from"../chunks/DropdownKvP.0dcf1200.js";import{M as Ll}from"../chunks/MissingValues.c08d686a.js";import{E as Rl}from"../chunks/ErrorMessage.13e1af6e.js";import{i as Bl,a as Pl,s as Ul,d as Ol}from"../chunks/types.1217ad3c.js";function ql(){return{}}const br=Object.freeze(Object.defineProperty({__proto__:null,load:ql},Symbol.toStringTag,{value:"Module"}));function Hl(r){let e,t,l,s,o,u,h,a,p,E,d,b,y,S,V,w,U,L,O,se,X,H;o=new pt({props:{icon:gl}}),p=new pt({props:{icon:nl}});function Ee(D){r[9](D)}let _e={model:r[0]};r[2]!==void 0&&(_e.valid=r[2]),y=new El({props:_e}),We.push(()=>Xe(y,"valid",Ee));function W(D){r[10](D)}function me(D){r[11](D)}function Ae(D){r[12](D)}let q={data:r[4]()};return r[0].variables!==void 0&&(q.variables=r[0].variables),r[1]!==void 0&&(q.valid=r[1]),r[0].missingValues!==void 0&&(q.missingValues=r[0].missingValues),w=new Dl({props:q}),We.push(()=>Xe(w,"variables",W)),We.push(()=>Xe(w,"valid",me)),We.push(()=>Xe(w,"missingValues",Ae)),{c(){e=g("div"),t=g("div"),l=g("div"),s=g("button"),ee(o.$$.fragment),u=A(),h=g("div"),a=g("button"),ee(p.$$.fragment),b=A(),ee(y.$$.fragment),V=A(),ee(w.$$.fragment),this.h()},l(D){e=_(D,"DIV",{});var B=m(e);t=_(B,"DIV",{class:!0});var N=m(t);l=_(N,"DIV",{class:!0});var j=m(l);s=_(j,"BUTTON",{id:!0,title:!0,class:!0});var De=m(s);te(o.$$.fragment,De),De.forEach(i),j.forEach(i),u=T(N),h=_(N,"DIV",{class:!0});var ce=m(h);a=_(ce,"BUTTON",{id:!0,title:!0,class:!0});var Z=m(a);te(p.$$.fragment,Z),Z.forEach(i),ce.forEach(i),N.forEach(i),b=T(B),te(y.$$.fragment,B),V=T(B),te(w.$$.fragment,B),B.forEach(i),this.h()},h(){c(s,"id","back"),c(s,"title","back"),c(s,"class","btn variant-filled-warning"),c(l,"class","grow"),c(a,"id","save"),c(a,"title","save"),c(a,"class","btn variant-filled-primary text-xl"),a.disabled=E=!r[1]||!r[2],c(h,"class","flex-none text-end"),c(t,"class","flex")},m(D,B){ge(D,e,B),n(e,t),n(t,l),n(l,s),le(o,s,null),n(t,u),n(t,h),n(h,a),le(p,a,null),n(e,b),le(y,e,null),n(e,V),le(w,e,null),se=!0,X||(H=[K(s,"mouseover",r[6]),K(s,"click",r[7]),K(a,"mouseover",r[8]),K(a,"click",r[3])],X=!0)},p(D,[B]){(!se||B&6&&E!==(E=!D[1]||!D[2]))&&(a.disabled=E);const N={};B&1&&(N.model=D[0]),!S&&B&4&&(S=!0,N.valid=D[2],Ze(()=>S=!1)),y.$set(N);const j={};!U&&B&1&&(U=!0,j.variables=D[0].variables,Ze(()=>U=!1)),!L&&B&2&&(L=!0,j.valid=D[1],Ze(()=>L=!1)),!O&&B&1&&(O=!0,j.missingValues=D[0].missingValues,Ze(()=>O=!1)),w.$set(j)},i(D){se||(C(o.$$.fragment,D),C(p.$$.fragment,D),ol(()=>{se&&(d||(d=$t(t,wt,{},!0)),d.run(1))}),C(y.$$.fragment,D),C(w.$$.fragment,D),se=!0)},o(D){R(o.$$.fragment,D),R(p.$$.fragment,D),d||(d=$t(t,wt,{},!1)),d.run(0),R(y.$$.fragment,D),R(w.$$.fragment,D),se=!1},d(D){D&&i(e),re(o),re(p),D&&d&&d.end(),re(y),re(w),X=!1,At(H)}}}function Fl(r,e,t){let l=!1,s=!1;il();let{model:o}=e;async function u(){const w=await Sl(o);console.log("save",w),o.entityId>0?St("/dcm/edit?id="+o.entityId):St("/rpm/datastructure")}function h(){let w=[];if(o&&o.markers){const U=o.markers.find(L=>L.type=="data");if(U)for(let L=U.row;L<o.preview.length;L++){const O=o.preview[L];w.push(O.split(String.fromCharCode(o.delimeter)))}}return w}function a(){St(document.referrer)}const p=()=>Ke.show("back"),E=()=>a(),d=()=>Ke.show("save");function b(w){s=w,t(2,s)}function y(w){r.$$.not_equal(o.variables,w)&&(o.variables=w,t(0,o))}function S(w){l=w,t(1,l)}function V(w){r.$$.not_equal(o.missingValues,w)&&(o.missingValues=w,t(0,o))}return r.$$set=w=>{"model"in w&&t(0,o=w.model)},r.$$.update=()=>{r.$$.dirty&1},[o,l,s,u,h,a,p,E,d,b,y,S,V]}class Gl extends Et{constructor(e){super(),Dt(this,e,Fl,Hl,yt,{model:0})}}function jl(r){let e,t,l,s,o,u,h,a,p,E,d,b,y,S,V,w,U,L,O,se,X,H,Ee,_e,W,me,Ae,q,D,B,N,j,De,ce,Z,Oe,qe,J,de,He,ie,ne,he,Re,Te,ye,be,ve,ue,Q,ae,pe,Fe,Ge,f,$;return{c(){e=g("div"),t=g("h4"),l=F("Controls"),s=A(),o=g("hr"),u=A(),h=g("dl"),a=g("div"),p=g("span"),E=A(),d=g("span"),b=g("dt"),y=F("Selection"),S=A(),V=g("dd"),w=F("left mouse button"),U=A(),L=g("div"),O=g("span"),se=A(),X=g("span"),H=g("dt"),Ee=F("Drag"),_e=A(),W=g("dd"),me=F("left mouse button down and drag"),Ae=A(),q=g("div"),D=g("span"),B=A(),N=g("span"),j=g("dt"),De=F("Deselect"),ce=A(),Z=g("dd"),Oe=F("right mouse button click"),qe=A(),J=g("div"),de=g("span"),He=A(),ie=g("span"),ne=g("dt"),he=F("Select Row"),Re=A(),Te=g("dd"),ye=F("double click left mouse button or click arrow"),be=A(),ve=g("div"),ue=g("span"),Q=A(),ae=g("span"),pe=g("dt"),Fe=F("Deselect Row"),Ge=A(),f=g("dd"),$=F("right mouse button click on arrow"),this.h()},l(k){e=_(k,"DIV",{class:!0});var M=m(e);t=_(M,"H4",{class:!0});var x=m(t);l=G(x,"Controls"),x.forEach(i),s=T(M),o=_(M,"HR",{class:!0}),u=T(M),h=_(M,"DL",{class:!0});var z=m(h);a=_(z,"DIV",{});var Se=m(a);p=_(Se,"SPAN",{class:!0}),m(p).forEach(i),E=T(Se),d=_(Se,"SPAN",{class:!0});var ke=m(d);b=_(ke,"DT",{class:!0});var Ne=m(b);y=G(Ne,"Selection"),Ne.forEach(i),S=T(ke),V=_(ke,"DD",{});var rt=m(V);w=G(rt,"left mouse button"),rt.forEach(i),ke.forEach(i),U=T(Se),Se.forEach(i),L=_(z,"DIV",{});var we=m(L);O=_(we,"SPAN",{class:!0}),m(O).forEach(i),se=T(we),X=_(we,"SPAN",{class:!0});var Be=m(X);H=_(Be,"DT",{class:!0});var st=m(H);Ee=G(st,"Drag"),st.forEach(i),_e=T(Be),W=_(Be,"DD",{});var nt=m(W);me=G(nt,"left mouse button down and drag"),nt.forEach(i),Be.forEach(i),Ae=T(we),we.forEach(i),q=_(z,"DIV",{});var Me=m(q);D=_(Me,"SPAN",{class:!0}),m(D).forEach(i),B=T(Me),N=_(Me,"SPAN",{class:!0});var je=m(N);j=_(je,"DT",{class:!0});var at=m(j);De=G(at,"Deselect"),at.forEach(i),ce=T(je),Z=_(je,"DD",{});var ze=m(Z);Oe=G(ze,"right mouse button click"),ze.forEach(i),je.forEach(i),qe=T(Me),Me.forEach(i),J=_(z,"DIV",{});var Ce=m(J);de=_(Ce,"SPAN",{class:!0}),m(de).forEach(i),He=T(Ce),ie=_(Ce,"SPAN",{class:!0});var Je=m(ie);ne=_(Je,"DT",{class:!0});var ot=m(ne);he=G(ot,"Select Row"),ot.forEach(i),Re=T(Je),Te=_(Je,"DD",{});var Pe=m(Te);ye=G(Pe,"double click left mouse button or click arrow"),Pe.forEach(i),Je.forEach(i),be=T(Ce),Ce.forEach(i),ve=_(z,"DIV",{});var Le=m(ve);ue=_(Le,"SPAN",{class:!0}),m(ue).forEach(i),Q=T(Le),ae=_(Le,"SPAN",{class:!0});var $e=m(ae);pe=_($e,"DT",{class:!0});var it=m(pe);Fe=G(it,"Deselect Row"),it.forEach(i),Ge=T($e),f=_($e,"DD",{});var ft=m(f);$=G(ft,"right mouse button click on arrow"),ft.forEach(i),$e.forEach(i),Le.forEach(i),z.forEach(i),M.forEach(i),this.h()},h(){c(t,"class","h4"),c(o,"class","divide-x-8"),c(p,"class","badge bg-primary-500"),c(b,"class","font-bold"),c(d,"class","flex-auto"),c(O,"class","badge bg-primary-500"),c(H,"class","font-bold"),c(X,"class","flex-auto"),c(D,"class","badge bg-primary-500"),c(j,"class","font-bold"),c(N,"class","flex-auto"),c(de,"class","badge bg-primary-500"),c(ne,"class","font-bold"),c(ie,"class","flex-auto"),c(ue,"class","badge bg-primary-500"),c(pe,"class","font-bold"),c(ae,"class","flex-auto"),c(h,"class","list-dl gap-0"),c(e,"class","col-span-1 card p-5 text-sm")},m(k,M){ge(k,e,M),n(e,t),n(t,l),n(e,s),n(e,o),n(e,u),n(e,h),n(h,a),n(a,p),n(a,E),n(a,d),n(d,b),n(b,y),n(d,S),n(d,V),n(V,w),n(a,U),n(h,L),n(L,O),n(L,se),n(L,X),n(X,H),n(H,Ee),n(X,_e),n(X,W),n(W,me),n(L,Ae),n(h,q),n(q,D),n(q,B),n(q,N),n(N,j),n(j,De),n(N,ce),n(N,Z),n(Z,Oe),n(q,qe),n(h,J),n(J,de),n(J,He),n(J,ie),n(ie,ne),n(ne,he),n(ie,Re),n(ie,Te),n(Te,ye),n(J,be),n(h,ve),n(ve,ue),n(ve,Q),n(ve,ae),n(ae,pe),n(pe,Fe),n(ae,Ge),n(ae,f),n(f,$)},p:ct,i:ct,o:ct,d(k){k&&i(e)}}}class zl extends Et{constructor(e){super(),Dt(this,e,null,jl,yt,{})}}function Qt(r,e,t){const l=r.slice();return l[54]=e[t],l[56]=t,l}function xt(r,e,t){const l=r.slice();return l[57]=e[t],l[59]=t,l}function el(r,e,t){const l=r.slice();return l[60]=e[t],l}function Yl(r){let e,t,l,s,o,u,h,a,p,E,d,b,y,S,V,w,U,L,O,se,X,H,Ee,_e,W,me,Ae,q,D,B,N,j,De,ce,Z,Oe,qe,J,de,He,ie,ne,he,Re,Te,ye,be,ve,ue,Q,ae,pe,Fe,Ge,f=r[0].total+"",$,k,M,x,z,Se,ke=r[0].total-r[0].skipped+"",Ne,rt,we,Be,st,nt,Me=r[0].skipped+"",je,at,ze,Ce,Je,ot,Pe,Le,$e,it,ft;function ul(v){r[18](v)}let Nt={id:"delimeter",title:"Delimeter",source:r[0].delimeters,complexTarget:!1,help:!0};r[0].delimeter!==void 0&&(Nt.target=r[0].delimeter),u=new Vt({props:Nt}),We.push(()=>Xe(u,"target",ul)),u.$on("change",r[15]);function fl(v){r[19](v)}let Mt={id:"decimal",title:"Decimal",source:r[0].decimals,complexTarget:!1,help:!0};r[0].decimal!==void 0&&(Mt.target=r[0].decimal),p=new Vt({props:Mt}),We.push(()=>Xe(p,"target",fl));function cl(v){r[20](v)}let Ct={id:"textMarker",title:"TextMarker",source:r[0].textMarkers,complexTarget:!1,help:!0};r[0].textMarker!==void 0&&(Ct.target=r[0].textMarker),b=new Vt({props:Ct}),We.push(()=>Xe(b,"target",cl)),j=new pt({props:{icon:_l}});function dl(v){r[32](v)}let Lt={};r[0].missingValues!==void 0&&(Lt.list=r[0].missingValues),Z=new Ll({props:Lt}),We.push(()=>Xe(Z,"list",dl));let ut=r[2],fe=[];for(let v=0;v<ut.length;v+=1)fe[v]=tl(el(r,ut,v));he=new pt({props:{icon:nl}}),be=new zl({});let tt=r[0].preview,Y=[];for(let v=0;v<tt.length;v+=1)Y[v]=rl(Qt(r,tt,v));const pl=v=>R(Y[v],1,1,()=>{Y[v]=null});return{c(){e=g("form"),t=g("div"),l=g("div"),s=g("div"),o=g("div"),ee(u.$$.fragment),a=A(),ee(p.$$.fragment),d=A(),ee(b.$$.fragment),S=A(),V=g("div"),w=g("button"),U=F("Variable"),L=A(),O=g("button"),se=F("Unit"),X=A(),H=g("button"),Ee=F("Description"),_e=A(),W=g("button"),me=F("Missing Values"),Ae=A(),q=g("button"),D=F("Data"),B=A(),N=g("button"),ee(j.$$.fragment),De=A(),ce=g("div"),ee(Z.$$.fragment),qe=A(),J=g("div"),de=g("div");for(let v=0;v<fe.length;v+=1)fe[v].c();He=A(),ie=g("div"),ne=g("button"),ee(he.$$.fragment),Te=A(),ye=g("div"),ee(be.$$.fragment),ve=A(),ue=g("div"),Q=g("div"),ae=g("label"),pe=g("b"),Fe=F("Total:"),Ge=A(),$=F(f),k=A(),M=g("label"),x=g("b"),z=F("Found:"),Se=A(),Ne=F(ke),rt=A(),we=g("label"),Be=g("b"),st=F("Skipped:"),nt=A(),je=F(Me),at=A(),ze=g("label"),Ce=g("i"),Je=F("you see only the first 10 rows of the data"),ot=A(),Pe=g("table"),Le=g("tbody");for(let v=0;v<Y.length;v+=1)Y[v].c();this.h()},l(v){e=_(v,"FORM",{});var P=m(e);t=_(P,"DIV",{id:!0,class:!0});var oe=m(t);l=_(oe,"DIV",{class:!0});var Qe=m(l);s=_(Qe,"DIV",{id:!0,class:!0});var Ve=m(s);o=_(Ve,"DIV",{id:!0,class:!0});var Ye=m(o);te(u.$$.fragment,Ye),a=T(Ye),te(p.$$.fragment,Ye),d=T(Ye),te(b.$$.fragment,Ye),Ye.forEach(i),S=T(Ve),V=_(Ve,"DIV",{id:!0,class:!0});var I=m(V);w=_(I,"BUTTON",{class:!0,id:!0,type:!0});var xe=m(w);U=G(xe,"Variable"),xe.forEach(i),L=T(I),O=_(I,"BUTTON",{class:!0,type:!0,id:!0});var Rt=m(O);se=G(Rt,"Unit"),Rt.forEach(i),X=T(I),H=_(I,"BUTTON",{class:!0,type:!0,id:!0});var Bt=m(H);Ee=G(Bt,"Description"),Bt.forEach(i),_e=T(I),W=_(I,"BUTTON",{class:!0,type:!0,color:!0,id:!0});var Pt=m(W);me=G(Pt,"Missing Values"),Pt.forEach(i),Ae=T(I),q=_(I,"BUTTON",{class:!0,type:!0,id:!0});var Ut=m(q);D=G(Ut,"Data"),Ut.forEach(i),B=T(I),N=_(I,"BUTTON",{title:!0,id:!0,class:!0,type:!0});var Ot=m(N);te(j.$$.fragment,Ot),Ot.forEach(i),I.forEach(i),De=T(Ve),ce=_(Ve,"DIV",{id:!0,class:!0});var qt=m(ce);te(Z.$$.fragment,qt),qt.forEach(i),qe=T(Ve),J=_(Ve,"DIV",{class:!0});var mt=m(J);de=_(mt,"DIV",{id:!0,class:!0});var Ht=m(de);for(let lt=0;lt<fe.length;lt+=1)fe[lt].l(Ht);Ht.forEach(i),He=T(mt),ie=_(mt,"DIV",{class:!0});var Ft=m(ie);ne=_(Ft,"BUTTON",{title:!0,class:!0});var Gt=m(ne);te(he.$$.fragment,Gt),Gt.forEach(i),Ft.forEach(i),mt.forEach(i),Ve.forEach(i),Te=T(Qe),ye=_(Qe,"DIV",{class:!0});var jt=m(ye);te(be.$$.fragment,jt),jt.forEach(i),Qe.forEach(i),ve=T(oe),ue=_(oe,"DIV",{id:!0,class:!0});var ht=m(ue);Q=_(ht,"DIV",{id:!0,class:!0});var et=m(Q);ae=_(et,"LABEL",{});var bt=m(ae);pe=_(bt,"B",{});var zt=m(pe);Fe=G(zt,"Total:"),zt.forEach(i),Ge=T(bt),$=G(bt,f),bt.forEach(i),k=T(et),M=_(et,"LABEL",{});var vt=m(M);x=_(vt,"B",{});var Yt=m(x);z=G(Yt,"Found:"),Yt.forEach(i),Se=T(vt),Ne=G(vt,ke),vt.forEach(i),rt=T(et),we=_(et,"LABEL",{});var kt=m(we);Be=_(kt,"B",{});var Kt=m(Be);st=G(Kt,"Skipped:"),Kt.forEach(i),nt=T(kt),je=G(kt,Me),kt.forEach(i),at=T(et),ze=_(et,"LABEL",{class:!0});var Wt=m(ze);Ce=_(Wt,"I",{});var Xt=m(Ce);Je=G(Xt,"you see only the first 10 rows of the data"),Xt.forEach(i),Wt.forEach(i),et.forEach(i),ot=T(ht),Pe=_(ht,"TABLE",{class:!0});var Zt=m(Pe);Le=_(Zt,"TBODY",{});var Jt=m(Le);for(let lt=0;lt<Y.length;lt+=1)Y[lt].l(Jt);Jt.forEach(i),Zt.forEach(i),ht.forEach(i),oe.forEach(i),P.forEach(i),this.h()},h(){c(o,"id","reader selections"),c(o,"class","flex flex-none gap-2"),c(w,"class","btn variant-filled-error"),c(w,"id","selectVar"),c(w,"type","button"),c(O,"class","btn variant-filled-success"),c(O,"type","button"),c(O,"id","selectUnit"),c(H,"class","btn variant-filled-warning"),c(H,"type","button"),c(H,"id","selectDescription"),c(W,"class","btn variant-filled-secondary"),c(W,"type","button"),c(W,"color","info"),c(W,"id","selectMissingValues"),c(q,"class","btn variant-filled-primary"),c(q,"type","button"),c(q,"id","selectData"),c(N,"title","reset selection"),c(N,"id","resetSelection"),c(N,"class","btn variant-filled-warning text-lg"),c(N,"type","button"),c(V,"id","markers"),c(V,"class","py-5 flex gap-1"),c(ce,"id","missingvalues"),c(ce,"class","grow"),c(de,"id","errors"),c(de,"class","m-2 text-sm grow text-right"),c(ne,"title","save"),c(ne,"class","btn variant-filled-primary text-lg"),ne.disabled=Re=!r[5],c(ie,"class","text-right"),c(J,"class","flex"),c(s,"id","edit"),c(s,"class","flex flex-col grow gap-2"),c(ye,"class","controls"),c(l,"class","flex gap-5"),c(ze,"class","grow text-right"),c(Q,"id","data infos"),c(Q,"class","flex flex-auto gap-5 pb-2"),c(Pe,"class","table table-compact"),c(ue,"id","preview data"),c(ue,"class","flex-col py-5"),c(t,"id","structure-suggestion-container"),c(t,"class","flex-col gap-3")},m(v,P){ge(v,e,P),n(e,t),n(t,l),n(l,s),n(s,o),le(u,o,null),n(o,a),le(p,o,null),n(o,d),le(b,o,null),n(s,S),n(s,V),n(V,w),n(w,U),n(V,L),n(V,O),n(O,se),n(V,X),n(V,H),n(H,Ee),n(V,_e),n(V,W),n(W,me),n(V,Ae),n(V,q),n(q,D),n(V,B),n(V,N),le(j,N,null),n(s,De),n(s,ce),le(Z,ce,null),n(s,qe),n(s,J),n(J,de);for(let oe=0;oe<fe.length;oe+=1)fe[oe]&&fe[oe].m(de,null);n(J,He),n(J,ie),n(ie,ne),le(he,ne,null),n(l,Te),n(l,ye),le(be,ye,null),n(t,ve),n(t,ue),n(ue,Q),n(Q,ae),n(ae,pe),n(pe,Fe),n(ae,Ge),n(ae,$),n(Q,k),n(Q,M),n(M,x),n(x,z),n(M,Se),n(M,Ne),n(Q,rt),n(Q,we),n(we,Be),n(Be,st),n(we,nt),n(we,je),n(Q,at),n(Q,ze),n(ze,Ce),n(Ce,Je),n(ue,ot),n(ue,Pe),n(Pe,Le);for(let oe=0;oe<Y.length;oe+=1)Y[oe]&&Y[oe].m(Le,null);$e=!0,it||(ft=[K(w,"click",r[21]),K(w,"mouseover",r[22]),K(O,"mouseover",r[23]),K(O,"click",r[24]),K(H,"mouseover",r[25]),K(H,"click",r[26]),K(W,"mouseover",r[27]),K(W,"click",r[28]),K(q,"mouseover",r[29]),K(q,"click",r[30]),K(N,"mouseover",r[31]),K(N,"click",r[12]),K(Pe,"contextmenu",Jl),K(t,"mousedown",r[7]),K(t,"mouseup",r[8]),K(e,"submit",kl(r[14]))],it=!0)},p(v,P){const oe={};P[0]&1&&(oe.source=v[0].delimeters),!h&&P[0]&1&&(h=!0,oe.target=v[0].delimeter,Ze(()=>h=!1)),u.$set(oe);const Qe={};P[0]&1&&(Qe.source=v[0].decimals),!E&&P[0]&1&&(E=!0,Qe.target=v[0].decimal,Ze(()=>E=!1)),p.$set(Qe);const Ve={};P[0]&1&&(Ve.source=v[0].textMarkers),!y&&P[0]&1&&(y=!0,Ve.target=v[0].textMarker,Ze(()=>y=!1)),b.$set(Ve);const Ye={};if(!Oe&&P[0]&1&&(Oe=!0,Ye.list=v[0].missingValues,Ze(()=>Oe=!1)),Z.$set(Ye),P[0]&4){ut=v[2];let I;for(I=0;I<ut.length;I+=1){const xe=el(v,ut,I);fe[I]?fe[I].p(xe,P):(fe[I]=tl(xe),fe[I].c(),fe[I].m(de,null))}for(;I<fe.length;I+=1)fe[I].d(1);fe.length=ut.length}if((!$e||P[0]&32&&Re!==(Re=!v[5]))&&(ne.disabled=Re),(!$e||P[0]&1)&&f!==(f=v[0].total+"")&&dt($,f),(!$e||P[0]&1)&&ke!==(ke=v[0].total-v[0].skipped+"")&&dt(Ne,ke),(!$e||P[0]&1)&&Me!==(Me=v[0].skipped+"")&&dt(je,Me),P[0]&69195){tt=v[0].preview;let I;for(I=0;I<tt.length;I+=1){const xe=Qt(v,tt,I);Y[I]?(Y[I].p(xe,P),C(Y[I],1)):(Y[I]=rl(xe),Y[I].c(),C(Y[I],1),Y[I].m(Le,null))}for(gt(),I=tt.length;I<Y.length;I+=1)pl(I);_t()}},i(v){if(!$e){C(u.$$.fragment,v),C(p.$$.fragment,v),C(b.$$.fragment,v),C(j.$$.fragment,v),C(Z.$$.fragment,v),C(he.$$.fragment,v),C(be.$$.fragment,v);for(let P=0;P<tt.length;P+=1)C(Y[P]);$e=!0}},o(v){R(u.$$.fragment,v),R(p.$$.fragment,v),R(b.$$.fragment,v),R(j.$$.fragment,v),R(Z.$$.fragment,v),R(he.$$.fragment,v),R(be.$$.fragment,v),Y=Y.filter(Boolean);for(let P=0;P<Y.length;P+=1)R(Y[P]);$e=!1},d(v){v&&i(e),re(u),re(p),re(b),re(j),re(Z),It(fe,v),re(he),re(be),It(Y,v),it=!1,At(ft)}}}function Kl(r){let e,t,l,s;const o=[Xl,Wl],u=[];function h(a,p){return!a[0]||a[3].length==0||a[4]==!1?0:1}return e=h(r),t=u[e]=o[e](r),{c(){t.c(),l=Ue()},l(a){t.l(a),l=Ue()},m(a,p){u[e].m(a,p),ge(a,l,p),s=!0},p(a,p){let E=e;e=h(a),e===E?u[e].p(a,p):(gt(),R(u[E],1,1,()=>{u[E]=null}),_t(),t=u[e],t?t.p(a,p):(t=u[e]=o[e](a),t.c()),C(t,1),t.m(l.parentNode,l))},i(a){s||(C(t),s=!0)},o(a){R(t),s=!1},d(a){u[e].d(a),a&&i(l)}}}function tl(r){let e,t=r[60]+"",l;return{c(){e=g("label"),l=F(t),this.h()},l(s){e=_(s,"LABEL",{class:!0});var o=m(e);l=G(o,t),o.forEach(i),this.h()},h(){c(e,"class","text-error-500")},m(s,o){ge(s,e,o),n(e,l)},p(s,o){o[0]&4&&t!==(t=s[60]+"")&&dt(l,t)},d(s){s&&i(e)}}}function ll(r){let e,t=(r[57]=r[57].replaceAll(String.fromCharCode(r[0].textMarker),""))+"",l,s,o;function u(...d){return r[33](r[56],r[59],...d)}function h(...d){return r[34](r[56],r[59],...d)}function a(...d){return r[35](r[56],r[59],...d)}function p(...d){return r[36](r[56],r[59],...d)}function E(...d){return r[37](r[56],r[59],...d)}return{c(){e=g("td"),l=F(t),this.h()},l(d){e=_(d,"TD",{class:!0});var b=m(e);l=G(b,t),b.forEach(i),this.h()},h(){var d,b,y,S,V;c(e,"class","hover:cursor-pointer select-none hover:border-surface-400 hover:border-solid hover:border-b-2"),Ie(e,"variant-soft-error",((d=r[1].find(u))==null?void 0:d.type)===r[6].VARIABLE),Ie(e,"variant-soft-success",((b=r[1].find(h))==null?void 0:b.type)===r[6].UNIT),Ie(e,"variant-soft-warning",((y=r[1].find(a))==null?void 0:y.type)===r[6].DESCRIPTION),Ie(e,"variant-soft-secondary",((S=r[1].find(p))==null?void 0:S.type)===r[6].MISSING_VALUES),Ie(e,"variant-soft-primary",((V=r[1].find(E))==null?void 0:V.type)===r[6].DATA),Ie(e,"variant-ghost-surface",r[3][r[56]][r[59]])},m(d,b){ge(d,e,b),n(e,l),s||(o=[K(e,"dblclick",r[11](r[56])),K(e,"mousedown",r[9](r[56],r[59])),K(e,"mouseenter",r[10](r[56],r[59]))],s=!0)},p(d,b){var y,S,V,w,U;r=d,b[0]&1&&t!==(t=(r[57]=r[57].replaceAll(String.fromCharCode(r[0].textMarker),""))+"")&&dt(l,t),b[0]&66&&Ie(e,"variant-soft-error",((y=r[1].find(u))==null?void 0:y.type)===r[6].VARIABLE),b[0]&66&&Ie(e,"variant-soft-success",((S=r[1].find(h))==null?void 0:S.type)===r[6].UNIT),b[0]&66&&Ie(e,"variant-soft-warning",((V=r[1].find(a))==null?void 0:V.type)===r[6].DESCRIPTION),b[0]&66&&Ie(e,"variant-soft-secondary",((w=r[1].find(p))==null?void 0:w.type)===r[6].MISSING_VALUES),b[0]&66&&Ie(e,"variant-soft-primary",((U=r[1].find(E))==null?void 0:U.type)===r[6].DATA),b[0]&8&&Ie(e,"variant-ghost-surface",r[3][r[56]][r[59]])},d(d){d&&i(e),s=!1,At(o)}}}function rl(r){let e,t,l,s,o,u,h,a,p;s=new pt({props:{icon:ml,size:"sm"}});let E=r[54].split(String.fromCharCode(r[0].delimeter)),d=[];for(let b=0;b<E.length;b+=1)d[b]=ll(xt(r,E,b));return{c(){e=g("tr"),t=g("td"),l=g("div"),ee(s.$$.fragment),o=A();for(let b=0;b<d.length;b+=1)d[b].c();u=A(),this.h()},l(b){e=_(b,"TR",{});var y=m(e);t=_(y,"TD",{class:!0});var S=m(t);l=_(S,"DIV",{class:!0});var V=m(l);te(s.$$.fragment,V),V.forEach(i),S.forEach(i),o=T(y);for(let w=0;w<d.length;w+=1)d[w].l(y);u=T(y),y.forEach(i),this.h()},h(){c(l,"class","pt-1"),c(t,"class","w-8 hover:cursor-pointer select-none text-sm hover:border-surface-400 hover:border-solid hover:border-b-2")},m(b,y){ge(b,e,y),n(e,t),n(t,l),le(s,l,null),n(e,o);for(let S=0;S<d.length;S+=1)d[S]&&d[S].m(e,null);n(e,u),h=!0,a||(p=K(t,"mousedown",r[16](r[56])),a=!0)},p(b,y){if(r=b,y[0]&3659){E=r[54].split(String.fromCharCode(r[0].delimeter));let S;for(S=0;S<E.length;S+=1){const V=xt(r,E,S);d[S]?d[S].p(V,y):(d[S]=ll(V),d[S].c(),d[S].m(e,u))}for(;S<d.length;S+=1)d[S].d(1);d.length=E.length}},i(b){h||(C(s.$$.fragment,b),h=!0)},o(b){R(s.$$.fragment,b),h=!1},d(b){b&&i(e),re(s),It(d,b),a=!1,p()}}}function Wl(r){let e,t,l;return t=new Tt({props:{position:al.center,label:"Generate Structure..."}}),{c(){e=g("div"),ee(t.$$.fragment),this.h()},l(s){e=_(s,"DIV",{class:!0});var o=m(e);te(t.$$.fragment,o),o.forEach(i),this.h()},h(){c(e,"class","h-full w-full text-surface-700")},m(s,o){ge(s,e,o),le(t,e,null),l=!0},p:ct,i(s){l||(C(t.$$.fragment,s),l=!0)},o(s){R(t.$$.fragment,s),l=!1},d(s){s&&i(e),re(t)}}}function Xl(r){let e,t,l;return t=new Tt({props:{position:al.center,label:"Loading Structure Suggestion based on: "+r[0].file}}),{c(){e=g("div"),ee(t.$$.fragment),this.h()},l(s){e=_(s,"DIV",{class:!0});var o=m(e);te(t.$$.fragment,o),o.forEach(i),this.h()},h(){c(e,"class","h-full w-full text-surface-700")},m(s,o){ge(s,e,o),le(t,e,null),l=!0},p(s,o){const u={};o[0]&1&&(u.label="Loading Structure Suggestion based on: "+s[0].file),t.$set(u)},i(s){l||(C(t.$$.fragment,s),l=!0)},o(s){R(t.$$.fragment,s),l=!1},d(s){s&&i(e),re(t)}}}function Zl(r){let e,t,l,s;const o=[Kl,Yl],u=[];function h(a,p){return!a[0]||a[3].length==0||a[4]==!1?0:1}return e=h(r),t=u[e]=o[e](r),{c(){t.c(),l=Ue()},l(a){t.l(a),l=Ue()},m(a,p){u[e].m(a,p),ge(a,l,p),s=!0},p(a,p){let E=e;e=h(a),e===E?u[e].p(a,p):(gt(),R(u[E],1,1,()=>{u[E]=null}),_t(),t=u[e],t?t.p(a,p):(t=u[e]=o[e](a),t.c()),C(t,1),t.m(l.parentNode,l))},i(a){s||(C(t),s=!0)},o(a){R(t),s=!1},d(a){u[e].d(a),a&&i(l)}}}const Jl=r=>r.preventDefault();function Ql(r,e,t){let{model:l}=e,{init:s=!0}=e,o=!1,u=[],h=[],a=0,p=0,E=!0,d=0,b=[];const y={VARIABLE:"variable",DESCRIPTION:"description",UNIT:"unit",MISSING_VALUES:"missing-values",DATA:"data"};let S=!1;const V=il();vl(async()=>{console.log("start selection suggestion"),console.log("load selection",l.entityId,l.file),w(l.preview,String.fromCharCode(l.delimeter)),U(l.markers,s),l.delimeter,N()});function w(f,$){console.log("set table infos"),a=f[0].split($).length,p=f.length,t(3,u=new Array(p).fill(!1));for(var k=0;k<u.length;k++)t(3,u[k]=new Array(a).fill(!1),u);console.log("state",u)}function U(f,$=!1){for(var k=0;k<f.length;k++){let M=f[k];console.log("marker",M),$?B(M.type,M.row-1,M.cells):B(M.type,M.row,M.cells)}}function L(f){o=!0}function O(f){o=!1}const se=(f,$)=>k=>{console.log(k.type),(k.which===3||k.button===2)&&console.log("Right mouse button at "+k.clientX+"x"+k.clientY),f!=d&&me(),(o||k.type==="mousedown")&&(d=f,(k.which===1||k.button===0)&&Ee($),(k.which===3||k.button===2)&&_e($))},X=(f,$)=>k=>{(o||k.type==="mousedown")&&((k.which===1||k.button===0)&&Ee($),(k.which===3||k.button===2)&&_e($))},H=f=>$=>{console.log("dblclick",$.type,o),(o||$.type==="dblclick")&&W(d)},Ee=f=>{t(3,u[d][f]=!0,u)},_e=f=>{if(t(3,u[d][f]=!1,u),console.log("deselect"),h.length>0){console.log("selection l:",h.length);for(let $=0;$<h.length;$++){const k=h[$];k&&(k.cells[f]=!1,B(k.type,k.row,k.cells))}}q(),N()},W=f=>{console.log("set true");for(var $=0;$<a;$++)console.log("select row",a),t(3,u[f][$]=!0,u)};function me(){t(3,u=new Array(p).fill(!1));for(var f=0;f<u.length;f++)t(3,u[f]=new Array(a).fill(!1),u)}function Ae(){t(1,h=[]),t(5,S=!1)}function q(){t(1,h=h.filter(f=>f.cells.find($=>$===!0)))}function D(f){let $=u[d];h.length>0&&($=h[0].cells),B(f,d,$),N()}function B(f,$,k){let M={type:f,row:$,cells:k},x=h.find(z=>z.row==M.row);x&&t(1,h=h.filter(z=>z.row!==M.row)),x=h.find(z=>z.type==M.type),x&&t(1,h=h.filter(z=>z.type!==M.type)),t(1,h=[...h,M])}function N(){var M;t(2,b=[]);let f=h.find(x=>x.type==y.VARIABLE),$=h.find(x=>x.type==y.DATA);if(h.length>0){f||b.push("the variables still need to be marked"),$||b.push("the data still need to be marked");let x=0;for(let z=0;z<h.length;z++){const Se=h[z];console.log(z,Se);let ke=(M=Se.cells.filter(Ne=>Ne==!0))==null?void 0:M.length;if(z>0&&ke!=x){let Ne="selection mismatch, the rows must have the same number of marked cells  ";b.push(Ne);break}x=ke}t(5,S=b.length==0)}else t(5,S=!1),t(2,b=[])}async function j(){if(t(4,E=!0),t(0,l.markers=h,l),l.entityId>0){console.log("save selection",l);let f=await Vl(l);console.log(f),f!=!1&&(console.log("selection",f),V("saved",l),t(4,E=!1))}else V("saved",l),t(4,E=!1)}function De(){w(l.preview,String.fromCharCode(l.delimeter))}const ce=f=>$=>{console.log(f,$.which,$.button),($.which===1||$.button===0)&&(me(),console.log(f),W(f),d=f),($.which===3||$.button===2)&&me()};function Z(f){r.$$.not_equal(l.delimeter,f)&&(l.delimeter=f,t(0,l))}function Oe(f){r.$$.not_equal(l.decimal,f)&&(l.decimal=f,t(0,l))}function qe(f){r.$$.not_equal(l.textMarker,f)&&(l.textMarker=f,t(0,l))}const J=()=>D(y.VARIABLE),de=()=>Ke.show("selectVar"),He=()=>Ke.show("selectUnit"),ie=()=>D(y.UNIT),ne=()=>Ke.show("selectDescription"),he=()=>D(y.DESCRIPTION),Re=()=>Ke.show("selectMissingValues"),Te=()=>D(y.MISSING_VALUES),ye=()=>Ke.show("selectData"),be=()=>D(y.DATA),ve=()=>Ke.show("resetSelection");function ue(f){r.$$.not_equal(l.missingValues,f)&&(l.missingValues=f,t(0,l))}const Q=(f,$,k)=>k.row===f&&k.cells[$]===!0,ae=(f,$,k)=>k.row===f&&k.cells[$]===!0,pe=(f,$,k)=>k.row===f&&k.cells[$]===!0,Fe=(f,$,k)=>k.row===f&&k.cells[$]===!0,Ge=(f,$,k)=>k.row===f&&k.cells[$]===!0;return r.$$set=f=>{"model"in f&&t(0,l=f.model),"init"in f&&t(17,s=f.init)},r.$$.update=()=>{r.$$.dirty[0]&1,r.$$.dirty[0]&2,r.$$.dirty[0]&4},[l,h,b,u,E,S,y,L,O,se,X,H,Ae,D,j,De,ce,s,Z,Oe,qe,J,de,He,ie,ne,he,Re,Te,ye,be,ve,ue,Q,ae,pe,Fe,Ge]}class xl extends Et{constructor(e){super(),Dt(this,e,Ql,Zl,yt,{model:0,init:17},null,[-1,-1,-1])}}function er(r){let e,t;return e=new Rl({props:{error:r[14]}}),{c(){ee(e.$$.fragment)},l(l){te(e.$$.fragment,l)},m(l,s){le(e,l,s),t=!0},p:ct,i(l){t||(C(e.$$.fragment,l),t=!0)},o(l){R(e.$$.fragment,l),t=!1},d(l){re(e,l)}}}function tr(r){let e,t,l=r[0]&&sl(r);return{c(){l&&l.c(),e=Ue()},l(s){l&&l.l(s),e=Ue()},m(s,o){l&&l.m(s,o),ge(s,e,o),t=!0},p(s,o){s[0]?l?(l.p(s,o),o&1&&C(l,1)):(l=sl(s),l.c(),C(l,1),l.m(e.parentNode,e)):l&&(gt(),R(l,1,1,()=>{l=null}),_t())},i(s){t||(C(l),t=!0)},o(s){R(l),t=!1},d(s){l&&l.d(s),s&&i(e)}}}function sl(r){let e,t,l,s;const o=[rr,lr],u=[];function h(a,p){return a[1]?0:a[0].variables?1:-1}return~(e=h(r))&&(t=u[e]=o[e](r)),{c(){t&&t.c(),l=Ue()},l(a){t&&t.l(a),l=Ue()},m(a,p){~e&&u[e].m(a,p),ge(a,l,p),s=!0},p(a,p){let E=e;e=h(a),e===E?~e&&u[e].p(a,p):(t&&(gt(),R(u[E],1,1,()=>{u[E]=null}),_t()),~e?(t=u[e],t?t.p(a,p):(t=u[e]=o[e](a),t.c()),C(t,1),t.m(l.parentNode,l)):t=null)},i(a){s||(C(t),s=!0)},o(a){R(t),s=!1},d(a){~e&&u[e].d(a),a&&i(l)}}}function lr(r){let e,t;return e=new Gl({props:{model:r[0]}}),e.$on("back",r[6]),{c(){ee(e.$$.fragment)},l(l){te(e.$$.fragment,l)},m(l,s){le(e,l,s),t=!0},p(l,s){const o={};s&1&&(o.model=l[0]),e.$set(o)},i(l){t||(C(e.$$.fragment,l),t=!0)},o(l){R(e.$$.fragment,l),t=!1},d(l){re(e,l)}}}function rr(r){let e,t,l,s,o;function u(a){r[7](a)}let h={init:r[2]};return r[0]!==void 0&&(h.model=r[0]),t=new xl({props:h}),We.push(()=>Xe(t,"model",u)),t.$on("saved",r[5]),{c(){e=g("div"),ee(t.$$.fragment)},l(a){e=_(a,"DIV",{});var p=m(e);te(t.$$.fragment,p),p.forEach(i)},m(a,p){ge(a,e,p),le(t,e,null),o=!0},p(a,p){const E={};p&4&&(E.init=a[2]),!l&&p&1&&(l=!0,E.model=a[0],Ze(()=>l=!1)),t.$set(E)},i(a){o||(C(t.$$.fragment,a),ol(()=>{o&&(s||(s=$t(e,wt,{},!0)),s.run(1))}),o=!0)},o(a){R(t.$$.fragment,a),s||(s=$t(e,wt,{},!1)),s.run(0),o=!1},d(a){a&&i(e),re(t),a&&s&&s.end()}}}function sr(r){let e,t;return e=new Tt({props:{label:r[3]}}),{c(){ee(e.$$.fragment)},l(l){te(e.$$.fragment,l)},m(l,s){le(e,l,s),t=!0},p(l,s){const o={};s&8&&(o.label=l[3]),e.$set(o)},i(l){t||(C(e.$$.fragment,l),t=!0)},o(l){R(e.$$.fragment,l),t=!1},d(l){re(e,l)}}}function nr(r){let e,t,l={ctx:r,current:null,token:null,hasCatch:!0,pending:sr,then:tr,catch:er,error:14,blocks:[,,,]};return wl(r[4](),l),{c(){e=Ue(),l.block.c()},l(s){e=Ue(),l.block.l(s)},m(s,o){ge(s,e,o),l.block.m(s,l.anchor=o),l.mount=()=>e.parentNode,l.anchor=e,t=!0},p(s,o){r=s,$l(l,r,o)},i(s){t||(C(l.block),t=!0)},o(s){for(let o=0;o<3;o+=1){const u=l.blocks[o];R(u)}t=!1},d(s){s&&i(e),l.block.d(s),l.token=null,l=null}}}function ar(r){let e,t;return e=new hl({props:{title:"Data structure",note:"This page allows you to create and edit a selected data structure.",contentLayoutType:bl.full,help:!0,$$slots:{default:[nr]},$$scope:{ctx:r}}}),{c(){ee(e.$$.fragment)},l(l){te(e.$$.fragment,l)},m(l,s){le(e,l,s),t=!0},p(l,[s]){const o={};s&32783&&(o.$$scope={dirty:s,ctx:l}),e.$set(o)},i(l){t||(C(e.$$.fragment,l),t=!0)},o(l){R(e.$$.fragment,l),t=!1},d(l){re(e,l)}}}function or(r,e,t){let l=yl,s,o,u=0,h=0,a,p,E=!0,d=!0,b="the structure is loading";async function y(){var X,H;Ke.setHelpItemList(l),s=document.getElementById("datastructure"),o=Number(s==null?void 0:s.getAttribute("dataset")),h=Number(s==null?void 0:s.getAttribute("version")),a=""+(s==null?void 0:s.getAttribute("file")),u=Number(s==null?void 0:s.getAttribute("structure")),a&&t(3,b="the file "+a+" is currently being analyzed");const U=((X=s==null?void 0:s.getAttribute("isTemplateRequired"))==null?void 0:X.toLocaleLowerCase())=="true";Bl.set(U);const L=((H=s==null?void 0:s.getAttribute("isMeaningRequired"))==null?void 0:H.toLocaleLowerCase())=="true";console.log("🚀 ~ file: +page.svelte:57 ~ start ~ isMeaningRequired:",L),Pl.set(L),console.log("start structure suggestion",o,h,a,u),console.log("🚀 ~ file: +page.svelte:69 ~ start ~ file:",a),a!=""?(console.log("file exist",a,o,0),t(0,p=await Il(a,o,0))):u>0?(console.log("copy structure"),t(0,p=await Al(u)),t(1,E=!1)):(console.log("empty structure"),t(0,p=await Tl()),t(1,E=!1));const O=await Nl();Ul.set(O);const se=await Ml();Ol.set(se)}async function S(U){console.log("update",U.detail),t(0,p=U.detail);let L=await Cl(U.detail);L!=!1&&(t(0,p=L),t(1,E=!1))}function V(){t(1,E=!0),t(2,d=!1)}function w(U){p=U,t(0,p)}return r.$$.update=()=>{r.$$.dirty&1},[p,E,d,b,y,S,V,w]}class vr extends Et{constructor(e){super(),Dt(this,e,or,ar,yt,{})}}export{vr as component,br as universal};
