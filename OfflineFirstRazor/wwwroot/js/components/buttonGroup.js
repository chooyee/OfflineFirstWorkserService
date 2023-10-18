
class ButtonGroup extends Base
{    
    constructor(jsonConfig) {   
        super();     
        this.group = "Basic"
        this.name = "ButtonGroup";
        this.version = "v1";
        if (jsonConfig!== void 0)        
          this.config = JSON.parse(jsonConfig);        
        else
          this.config =  {};       
     
        //console.log(this.config)
        let config = this.config;
        if (config.propertyName === void 0) { config.propertyName = ''; }
        if (config.type === void 0) { config.type = 'ButtonGroup'; }
        if (config.buttonLabel === void 0) { config.buttonLabel = 'My ButtonGroup'; }
        if (config.buttonPostUrl === void 0) { config.buttonPostUrl = ''; }
        if (config.buttonActionType=== void 0) { config.buttonActionType = 'Submit'; }
       
        if (config.validation === void 0) {
          config.validation = {}
          config.validation.mandatory = false;

          if (config.validation.lengthCheck === void 0) {
            config.validation.lengthCheck = {};
            config.validation.lengthCheck.min = 0;
            config.validation.lengthCheck.max = 0;
          };
        };
        
      
        if (config.formBuilderMode === void 0) { config.formBuilderMode = false; }
        //this.editFormJson = this.getEditFormJson();
    }

    getEditFormJson = () =>{
      return {
        "label": "Text Field Component Edit Form",
        "tabs": [
           {"tabLabel": "Display",
             "tabId":"home",
            "components": [              
              {
                "propertyName":"buttonLabel",
                "label": "Button Label",
                "placeholder": "My Button label",
                "description": "Enter the label for this Button", 
                "validation":{"mandatory": true, "lengthCheck": {"min": 0, "max":0}},
                "value":this.config.buttonLabel,           
                "type": "Textbox"
              },
              {
                "propertyName":"buttonLabel",
                "label": "Button Label",
                "placeholder": "My Button label",
                "description": "Enter the label for this Button", 
                "validation":{"mandatory": true, "lengthCheck": {"min": 0, "max":0}},
                "value":this.config.buttonLabel,           
                "type": "Textbox"
              },
              {
                "propertyName":"buttonClass",
                "label": "Button Color Type",
                "value":this.config.buttonClass,       
                "description": "Select the type for the button", 
                "options":[
                {"primary":"primary"},
                {"secondary":"secondary"},
                {"success":"success"},
                {"danger":"danger"},
                {"warning":"warning"},
                {"info":"info"},
                {"light":"light"},
                {"dark":"dark"}
                ],    
                "type": "Select"
              },        
              {
                "propertyName":"buttonSize",
                "label": "Button Size",
                "value":this.config.buttonSize,       
                "description": "Select the size for the button", 
                "options":[
                {"md":"Medium"},
                {"lg":"Large"},
                {"sm":"Small"}
                ],    
                "type": "Select"
              },        
              {
                "propertyName":"position",
                "label": "Header Position",
                "value":this.config.position,       
                "description": "Select the position for the header", 
                "options":[
                {"left":"left"},
                {"center":"center"},
                {"right":"right"}
                ],    
                "type": "Select"
              }, 
              {
                "propertyName":"customCss",
                "label": "Custom CSS",
                "placeholder": "custom css",
                "description": "Set the custom css, for ex: form-control form-item", 
                "type": "Textbox",
                "value": this.config.customCss
              }
            ]
          },           
          {
            "tabLabel": "Behavior",
            "tabId":"behavior",
            "components": [
              {
                "propertyId":"editFormButtonActionType",
                "propertyName":"buttonActionType",
                "label": "Button Action", 
                "options":[
                  {"Submit":"Submit"},
                  {"Reset":"Reset"},
                  {"Post":"Post"},
                  {"Navigate":"Navigate"}
                ],
                "value": this.config.buttonActionType,           
                "type": "Select"
              },
              {
                "propertyId":"editFormButtonActionPostUrlTextbox",
                "propertyName":"buttonPostUrl",
                "label": "Post Url",
                "value": this.config.buttonPostUrl, 
                "type": "Textbox",
                "display":"none", 
                "prefix":"https://",
                "description":"ex: <pre>api.alliancelugin.io/dynamicform/xsddsse</pre>",               
                "events":[
                  {
                    "type":"editFormDisplay",                    
                    "target":"editFormButtonActionType",
                    "event":"change",
                    "handler":"editFormButtonActionTypeChangeListener"
                  }
                ]
              }
            ]
          },
          {
            "tabLabel": "Data",
            "tabId":"data",
            "components": [
              {
                "propertyName":"propertyName",
                "label": "Data Property",
                "value": this.config.propertyName, 
                "type": "Textbox"
              }
            ]
          }
        ]
      };
    }

    renderElement = (config, elementId)=>{

      //===================================================================================
        //render Input Group
        //===================================================================================
       
        const divInputGroup = this.createElement("div", {"class":"d-flex"});       
        if (config.position ==='center')
        {
          divInputGroup.classList.add("justify-content-center", "align-items-center")
        }
        else if (config.position ==='right')
        {
          divInputGroup.classList.add("justify-content-end", "align-items-right")
        }
        
        //<input aria-required="true" aria-labelledby="l-epx0fbk-textField d-epx0fbk-textField" id="epx0fbk-textField" value="" spellcheck="true" placeholder="Placeholder" autocomplete="off" lang="en" class="form-control" type="text" name="data[textField]" ref="input">
       
        const inputEl = this.renderRawElement(config, elementId);
        
        divInputGroup.appendChild(inputEl);
       
        const divElement = this.createElement("div", {"ref":"element"});
        divElement.appendChild(divInputGroup);

        return divElement;
        //===================================================================================
        //end render Input Group
        //===================================================================================
    }

    renderRawElement = (config, elementId)=>{   
      const propertyName = config.propertyName;

      const inputEl =  this.createElement("button", {"id":`${this.name}-${elementId}`, "class":"btn", "type":"text", "ref":"button"});
      inputEl.classList.add(`btn-${config.buttonClass}`)

      inputEl.classList.add(`btn-${config.buttonSize}`)

      inputEl.innerHTML = config.buttonLabel;

      //Must implement!
      if (!this.isNullOrEmpty(config.propertyId))
      {
        inputEl.setAttribute("data-id", config.propertyId);
      }

      if (!this.isNullOrEmpty(propertyName))
      {
        inputEl.setAttribute("data-property",propertyName);
      }
      
      
       
      return inputEl;
    }



}
