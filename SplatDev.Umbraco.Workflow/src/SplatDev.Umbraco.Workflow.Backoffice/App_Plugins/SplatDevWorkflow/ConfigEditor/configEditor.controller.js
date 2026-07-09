angular.module("umbraco").controller("SplatDevWorkflow.ConfigEditorController",["$scope","$http",function(s,h){
s.loading=false;s.saving=false;s.workflows=[];s.editingWorkflow=false;s.editor=null;s.toast=null;
s.themes=[{key:"default",name:"Default"},{key:"high-contrast",name:"High Contrast"},{key:"playful",name:"Playful"}];
var A="/umbraco/api/Workflow";
function t(m,y){s.toast={message:m,type:y||"info"};setTimeout(function(){s.$apply(function(){s.toast=null})},3500)}
function L(){s.loading=true;h.get(A+"/definitions").then(function(r){s.workflows=r.data||[];s.workflows.forEach(function(w){if(typeof w.definitionJson==="string"){try{w._def=JSON.parse(w.definitionJson)}catch(e){w._def={}}}else{w._def=w.definitionJson||{}}})}).catch(function(){t("Failed to load workflows","error")}).finally(function(){s.loading=false})}
L();
s.createNew=function(){s.editor={isExisting:false,workflow:{key:"",label:"",steps:[],themeKey:"default",queueColumns:[]}};s.editingWorkflow=true}
s.editWorkflow=function(w){var d=w._def||{};s.editor={isExisting:true,existingWorkflow:w,workflow:{key:w.key,label:w.label,steps:d.steps||[],themeKey:d.themeKey||"default",queueColumns:d.queueColumns||[]}};s.editingWorkflow=true}
s.cancelEdit=function(){s.editingWorkflow=false;s.editor=null}
s.addStep=function(){s.editor.workflow.steps.push({key:"",label:"",department:"",group:"",actions:[],preActionMessagesStr:"",postActionMessagesStr:""})}
s.removeStep=function(i){s.editor.workflow.steps.splice(i,1)}
s.moveStep=function(i,d){var n=i+d;if(n<0||n>=s.editor.workflow.steps.length)return;var x=s.editor.workflow.steps[i];s.editor.workflow.steps[i]=s.editor.workflow.steps[n];s.editor.workflow.steps[n]=x}
s.addAction=function(st){st.actions.push({key:"",label:"",nextStepKey:"",assignment:0})}
s.removeAction=function(st,i){st.actions.splice(i,1)}
s.addQueueColumn=function(){s.editor.workflow.queueColumns=s.editor.workflow.queueColumns||[];s.editor.workflow.queueColumns.push({header:"",fieldKey:"",width:""})}
s.removeQueueColumn=function(i){s.editor.workflow.queueColumns.splice(i,1)}
s.saveWorkflow=function(){if(!s.editor.workflow.label||!s.editor.workflow.key){t("Label and Key are required","error");return}
s.editor.workflow.steps.forEach(function(st){st.preActionMessages=(st.preActionMessagesStr||"").split(",").map(function(x){return{alias:x.trim(),label:x.trim(),audience:0}}).filter(function(m){return m.alias});st.postActionMessages=(st.postActionMessagesStr||"").split(",").map(function(x){return{alias:x.trim(),label:x.trim(),audience:0}}).filter(function(m){return m.alias})});
var p={key:s.editor.workflow.key,label:s.editor.workflow.label,definitionJson:JSON.stringify({steps:s.editor.workflow.steps,themeKey:s.editor.workflow.themeKey,queueColumns:s.editor.workflow.queueColumns})};s.saving=true;
var pr=s.editor.isExisting?h.put(A+"/definitions/"+s.editor.existingWorkflow.key,p):h.post(A+"/definitions",p);
pr.then(function(){t("Workflow saved","success");s.editingWorkflow=false;s.editor=null;L()}).catch(function(e){t(e.data&&e.data.detail||"Failed to save","error")}).finally(function(){s.saving=false})}
s.deleteWorkflow=function(w){if(!confirm("Delete '"+w.label+"'? This cannot be undone."))return;h.delete(A+"/definitions/"+w.key).then(function(){t("Workflow deleted","success");L()}).catch(function(e){t(e.data&&e.data.detail||"Failed to delete","error")})}
s.activateWorkflow=function(w){h.put(A+"/definitions/"+w.key+"/activate").then(function(){t("Workflow activated","success");L()}).catch(function(e){t(e.data&&e.data.detail||"Failed to activate","error")})}
}]);