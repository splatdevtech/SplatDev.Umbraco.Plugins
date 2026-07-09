angular.module("umbraco").controller("SplatDevWorkflow.QueueViewController",["$scope","$http",function(s,h){
s.loading=false;s.workflows=[];s.instances=[];s.selectedInstance=null;s.detailInstance=null;s.toast=null;s.activeTheme="default";
s.filters={workflowKey:"",status:"",assignedToMe:"",search:""};s.currentPage=1;s.pageSize=20;s.totalCount=0;s.totalPages=1;
s.activeColumns=[{header:"ID",fieldKey:"id",width:"60px"},{header:"Workflow",fieldKey:"workflowKey",width:""}];
var A="/umbraco/api/Workflow";
function t(m,y){s.toast={message:m,type:y||"info"};setTimeout(function(){s.$apply(function(){s.toast=null})},3500)}
h.get(A+"/definitions").then(function(r){s.workflows=r.data||[]});
s.loadInstances=function(){s.loading=true;var p={page:s.currentPage,pageSize:s.pageSize};if(s.filters.workflowKey)p.workflowKey=s.filters.workflowKey;if(s.filters.status!=="")p.status=s.filters.status;if(s.filters.assignedToMe)p.assignedToMe=s.filters.assignedToMe;if(s.filters.search)p.search=s.filters.search;h.get(A+"/instances",{params:p}).then(function(r){s.instances=r.data.items||[];s.totalCount=r.data.total||0;s.totalPages=Math.max(1,Math.ceil(s.totalCount/s.pageSize))}).catch(function(){t("Failed to load","error")}).finally(function(){s.loading=false})};
s.applyFilters=function(){s.currentPage=1;s.loadInstances()}
s.goToPage=function(p){s.currentPage=p;s.loadInstances()}
s.refresh=function(){s.loadInstances()}
s.selectInstance=function(i){s.selectedInstance=i}
s.openDetail=function(i){h.get(A+"/instances/"+i.id).then(function(r){s.detailInstance=r.data;s.selectedInstance=i}).catch(function(){t("Failed to load details","error")})}
s.closeDetail=function(){s.detailInstance=null}
s.loadInstances()
}]);