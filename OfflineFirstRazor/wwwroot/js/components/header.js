class Header extends Base
{    
    constructor(jsonConfig) {   
        super();     
        this.group = "Basic"
        this.name = "Header";
        this.version = "v1";
        if (jsonConfig!== void 0)        
          this.config = JSON.parse(jsonConfig);        
        else
          this.config =  {};       
     
        //console.log(this.config)
        let config = this.config;
        if (config.propertyName === void 0) { config.propertyName = ''; }
        if (config.type === void 0) { config.type = 'Header'; }
        if (config.label === void 0) { config.label = ''; }
        //if (this.isPropExists(config, 'labelPosition')) { config.labelPosition = 'top'; }
        if (config.labelWidth === void 0) { config.labelWidth = 30; }
        if (config.labelMargin === void 0) { config.labelMargin = 3; }
        if (config.value === void 0) { config.value = ""; }
        if (config.leftterCase === void 0) { config.leftterCase = "none"; }
       
        
        if (config.visibility === void 0) {
          config.visibility = {};

          if (config.visibility.simple === void 0) {
            config.visibility.simple = {};
            config.visibility.simple.target = "";
            config.visibility.simple.value = "";
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
                "propertyName":"headerType",
                "label": "Header Type",
                "value":this.config.headerType,       
                "description": "Select the header type", 
                "options":[
                {"h1":"H1"},
                {"h2":"H2"},
                {"h3":"H3"},
                {"h4":"H4"},
                {"h5":"H5"}
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
                "propertyName":"value",
                "label": "My Header",
                "description": "Enter the header",
                "value":this.config.value,           
                "type": "Textbox"
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
              {
                "propertyName":"description",
                "label": "Descriptions (Optional)",
                "description": "Description for the textbox", 
                "type": "Textbox",                
                "value": this.config.description
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
        //<input aria-required="true" aria-labelledby="l-epx0fbk-textField d-epx0fbk-textField" id="epx0fbk-textField" value="" spellcheck="true" placeholder="Placeholder" autocomplete="off" lang="en" class="form-control" type="text" name="data[textField]" ref="input">
       
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

      const inputEl =  this.createElement(this.config.headerType, {"id":`${this.name}-${elementId}`, "class":"mb-3", "ref":"input"});
      
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

    //============================================================================
    //Custom Event Listener
    //===========================================================================

}
