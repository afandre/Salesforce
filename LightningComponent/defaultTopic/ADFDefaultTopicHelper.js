({
   searchHelper : function(component,event,getInputkeyWord) {
       // call the apex class method in ADF_topicController.apxc
       var action = component.get("c.getNavigationTopics");
       //console.log(action);
       // set a callBack    
       action.setParams({
           'searchKeyWord': getInputkeyWord
       });      
       action.setCallback(this, function(response) {
           $A.util.removeClass(component.find("mySpinner"), "slds-show");
           var state = response.getState();
           if (state === "SUCCESS") {
               var responseValue = response.getReturnValue();
               // if storeResponse size is equal 0 ,display No Result Found... message on screen.                }
               if (responseValue.length == 0) {
                   component.set("v.Message", 'No Result Found...');
               } else {
                   component.set("v.Message", '');
               }
               // set searchResult list with return value from server.
               component.set("v.listOfSearchRecords", responseValue);
           }
           
       });
       // enqueue the Action  
       $A.enqueueAction(action);
       
   },
   setTopicHelper : function(component,event,topic) {
       $A.util.removeClass(component.find("dtSpinner"), "slds-hide");
       var action = component.get("c.setDefaultTopic");
       //component.set("v.Message", "Your selected default topic is " + topic);
       action.setParams({
           'defaultTopic': topic
       });      
       action.setCallback(this, function(response) {
           var state = response.getState();
           if (state === "SUCCESS") {
               var responseValue = response.getReturnValue();
               var toastEvent = $A.get("e.force:showToast");
               toastEvent.setParams({
                   "title": "Success!",
                   "message": "Your default product has been updated",
                   "type": "success"
               });
               toastEvent.fire();
               $A.util.addClass(component.find("dtSpinner"), "slds-hide");
               //console.dir(responseValue);
               //component.set("v.label", "Your Topic: " + responseValue.id);
           }            
           
       });
       
       $A.enqueueAction(action);
   },
   getTopicHelper : function(component,event) {
       var action = component.get("c.getDefaultTopic");
       action.setCallback(this, function(response) {
           var state = response.getState();
           if (state === "SUCCESS") {
               var responseValue = response.getReturnValue();
               component.set("v.selectedRecord" , responseValue);
               component.set("v.SearchKeyWord" , responseValue.name);
               component.set("v.selectedLookupRecord" , responseValue);
               component.set("v.selectedTopicId" , responseValue.id);
               //component.set("v.label", "Your Topic: " + responseValue.name);

           }           
       });
       $A.enqueueAction(action);

   },
   
})