({
    
   onfocus : function(component,event,helper){
       $A.util.addClass(component.find("mySpinner"), "slds-show");
       var forOpen = component.find("searchRes");
       $A.util.addClass(forOpen, 'slds-is-open');
       $A.util.removeClass(forOpen, 'slds-is-close');
       var getInputkeyWord = '';
       helper.searchHelper(component,event,getInputkeyWord);
   },    
   keyPressController : function(component, event, helper) {
       // get the search Input keyword   
       var getInputkeyWord = component.get("v.SearchKeyWord");
       // check if getInputKeyWord size id more then 0 then open the lookup result List and 
       // call the helper 
       // else close the lookup result List part.   
       if( getInputkeyWord.length > 0 ){
           var forOpen = component.find("searchRes");
           $A.util.addClass(forOpen, 'slds-is-open');
           $A.util.removeClass(forOpen, 'slds-is-close');
           helper.searchHelper(component,event,getInputkeyWord);
       }
       else{  
           component.set("v.listOfSearchRecords", null ); 
           var forclose = component.find("searchRes");
           $A.util.addClass(forclose, 'slds-is-close');
           $A.util.removeClass(forclose, 'slds-is-open');
       }
   },    
   onblur : function(component,event,helper){       
       component.set("v.listOfSearchRecords", null );
       var forclose = component.find("searchRes");
       $A.util.addClass(forclose, 'slds-is-close');
       $A.util.removeClass(forclose, 'slds-is-open');
   },    
   clear :function(component,event,heplper){
       var pillTarget = component.find("lookup-pill");
       var lookUpTarget = component.find("lookupField"); 
       
       $A.util.addClass(pillTarget, 'slds-hide');
       $A.util.removeClass(pillTarget, 'slds-show');
       
       $A.util.addClass(lookUpTarget, 'slds-show');
       $A.util.removeClass(lookUpTarget, 'slds-hide');
       
       component.set("v.SearchKeyWord",null);
       component.set("v.listOfSearchRecords", null );
       component.set("v.selectedRecord", {} );   
   },  
   // This function call when the end User Select any record from the result list.   
   handleComponentEvent : function(component, event, helper) {
       // get the selected record from the COMPONENT event
       var selectedAccountGetFromEvent = event.getParam("recordByEvent");
       component.set("v.selectedRecord" , selectedAccountGetFromEvent); 
       component.set("v.selectedTopicId", selectedAccountGetFromEvent.id);
       
       
       var forclose = component.find("lookup-pill");
       $A.util.addClass(forclose, 'slds-show');
       $A.util.removeClass(forclose, 'slds-hide');
       
       var forclose2 = component.find("searchRes");
       $A.util.addClass(forclose2, 'slds-is-close');
       $A.util.removeClass(forclose2, 'slds-is-open');
       
       var lookUpTarget = component.find("lookupField");
       $A.util.addClass(lookUpTarget, 'slds-hide');
       $A.util.removeClass(lookUpTarget, 'slds-show');  
       
   },
   topicSelect : function(component, event, helper){
       $A.util.removeClass(component.find("dtSpinnner"), "slds-hide");
       var topicId = component.get("v.selectedTopicId");
       helper.setTopicHelper(component,event,topicId);
   }, 
   doInit : function(component, event, helper){
       helper.getTopicHelper(component,event,helper);
   }
   
})