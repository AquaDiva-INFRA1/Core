import{a4 as n,A as r}from"./Api.js";function l(t){return t.style.display="block",{duration:n(t),tick:e=>{e===0&&t.classList.add("show")}}}function p(t){return t.classList.remove("show"),{duration:n(t),tick:e=>{e===0&&(t.style.display="none")}}}function y(t,s){const e=s.horizontal?"width":"height";return t.style[e]=`${t.getBoundingClientRect()[e]}px`,t.classList.add("collapsing"),t.classList.remove("collapse","show"),{duration:n(t),tick:a=>{a>0?t.style[e]="":a===0&&(t.classList.remove("collapsing"),t.classList.add("collapse"))}}}function u(t,s){const e=s.horizontal,o=e?"width":"height";return t.classList.add("collapsing"),t.classList.remove("collapse","show"),t.style[o]=0,{duration:n(t),tick:c=>{c<1?e?t.style.width=`${t.scrollWidth}px`:t.style.height=`${t.scrollHeight}px`:(t.classList.remove("collapsing"),t.classList.add("collapse","show"),t.style[o]="")}}}function d(t){return t.style.display="block",{duration:n(t),tick:e=>{e>0&&t.classList.add("show")}}}function m(t){return t.classList.remove("show"),{duration:n(t),tick:e=>{e===1&&(t.style.display="none")}}}const g=async t=>{console.log("edit");try{return(await r.get("/dcm/edit/load?id="+t)).data}catch(s){console.error(s)}},h=async(t,s,e)=>{try{const o=t+"?id="+s+"&version="+e;return(await r.get(o)).data}catch(o){console.error(o)}},w=async(t,s,e,o)=>{try{return(await r.post(t,{id:s,file:e,description:o})).data}catch(a){console.error(a)}},f=async(t,s,e)=>{try{return console.log("remove"),console.log(t),console.log(s),console.log(e),await r.post(t,{id:s,file:e})}catch(o){console.error(o)}},k=async t=>{try{return(await r.get("/dcm/entitytemplates/Get?id="+t)).data}catch(s){console.error(s)}},L=async()=>{try{return(await r.get("/dcm/entitytemplates/Load")).data}catch(t){console.error(t)}},v=async()=>{try{return(await r.get("/dcm/entitytemplates/Entities")).data}catch(t){console.error(t)}},E=async()=>{try{return(await r.get("/dcm/entitytemplates/MetadataStructures")).data}catch(t){console.error(t)}},S=async()=>{try{return(await r.get("/dcm/entitytemplates/SystemKeys")).data}catch(t){console.error(t)}},T=async()=>{try{return(await r.get("/dcm/entitytemplates/DataStructures")).data}catch(t){console.error(t)}},b=async()=>{try{return(await r.get("/dcm/entitytemplates/Hooks")).data}catch(t){console.error(t)}},D=async()=>{try{return(await r.get("/dcm/entitytemplates/Groups")).data}catch(t){console.error(t)}},x=async()=>{try{return(await r.get("/dcm/entitytemplates/FileTypes")).data}catch(t){console.error(t)}},F=async t=>{try{return(await r.post("/dcm/entitytemplates/Update",t)).data}catch(s){console.error(s)}},H=async t=>{try{return(await r.post("/dcm/entitytemplates/Delete?id="+t)).data}catch(s){console.error(s)}};export{S as a,y as b,u as c,l as d,p as e,m as f,L as g,h,g as i,k as j,F as k,H as l,d as m,b as n,E as o,T as p,v as q,f as r,w as s,D as t,x as u};
