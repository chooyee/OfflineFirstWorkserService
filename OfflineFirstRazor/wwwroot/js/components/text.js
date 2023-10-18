class Text extends Base
{    
    constructor(jsonConfig) {   
        super();     
        this.group = "Basic"
        this.name = "Text";
        this.version = "v1";
        if (jsonConfig!== void 0)        
          this.config = JSON.parse(jsonConfig);        
        else
          this.config =  {};       
     
        //console.log(this.config)
        let config = this.config;
        if (config.propertyName === void 0) { config.propertyName = ''; }
        if (config.type === void 0) { config.type = 'Text'; }
        if (config.position === void 0) { config.position = 'left'; }
       
        if (config.value === void 0) { config.value = ""; }
        if (config.letterCase === void 0) { config.letterCase = "none"; }
       
      

        if (config.formBuilderMode === void 0) { config.formBuilderMode = false; }
        //this.editFormJson = this.getEditFormJson();
    }

    getEditFormJson = () =>{
      return {
        "label": "Text Component Edit Form",
        "tabs": [
           {"tabLabel": "Display",
             "tabId":"home",
            "components": [              
              {
                "propertyName":"value",
                "label": "Text",
                "placeholder": "My Textbox label",
                "description": "Enter the text",
                "value":this.config.value,           
                "type": "Textarea"
              },
              {
                "propertyName":"position",
                "label": "Text Position",
                "description": "Set the position of the text", 
                "options":[
                    {"left":"left"},
                    {"center":"center"},
                    {"right":"right"}
                ],    
                "value": this.config.position,           
                "type": "Select"
              },
              {
                "propertyName":"letterCase",
                "label": "Lettercase",
                "description": "Select the letter case for the header",
                "value":this.config.letterCase, 
                "options":[
                  {"none":"None"},
                  {"upper":"Uppercase"},
                  {"lower":"Lowercase"},
                  {"capital":"Capital"}
                ],    
                "type": "Select"
              },    
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
  
        const desc = config.description;
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
          
          const inputEl = this.renderRawElement(config, elementId);
          
          divInputGroup.appendChild(inputEl);
  
          const divElement = this.createElement("div", {"ref":"element"});
          divElement.appendChild(divInputGroup);
  
          if (!this.isNullOrEmpty(desc))
          {
            //<div class="form-text text-muted" id="d-egxfg08-checkbox">testew er ewrewr ewrewr ewre w</div>
            const divDesc = this.createElement("div", {"class":"d-flex form-text text-muted"});
            if (config.position ==='center')
            {
              divDesc.classList.add("justify-content-center", "align-items-center")
            }
            else if (config.position ==='right')
            {
              divDesc.classList.add("justify-content-end", "align-items-right")
            }
            //const pDesc = this.createElement("p", {"class":"form-text text-muted"});
            divDesc.innerHTML = desc;
            //divDesc.appendChild(pDesc);
            divElement.appendChild(divDesc);
          }
  
          return divElement;
          //===================================================================================
          //end render Input Group
          //===================================================================================
      }
  
      renderRawElement = (config, elementId)=>{   
        const propertyName = config.propertyName;
        const letterCase = config.letterCase;    
        const val = config.value;
  
        const inputEl =  this.createElement("p", {"id":`${this.name}-${elementId}`, "ref":"input"});
        
        //Must implement!
        if (!this.isNullOrEmpty(config.propertyId))
        {
          inputEl.setAttribute("data-id", config.propertyId);
        }
  
        if (!this.isNullOrEmpty(propertyName))
        {
          inputEl.setAttribute("data-property",propertyName);
        }
        
        if (!this.isNullOrEmpty(val))
        {
          inputEl.innerHTML= val;
        }
  
        switch (letterCase)
        {
          case "upper":
            inputEl.classList.add("text-uppercase");
            break;
          case "lower":
            inputEl.classList.add("text-lowercase");
            break;
          case "capital":
            inputEl.classList.add("text-capitalize");
            break;
        }
  
        return inputEl;
      }
  
    
}
