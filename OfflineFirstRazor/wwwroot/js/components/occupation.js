class Occupation extends Base
{    
    constructor(jsonConfig) {   
        super();     
        this.group = "Custom"
        this.name = "Occupation";
        this.version = "v1";
        if (jsonConfig!== void 0)        
          this.config = JSON.parse(jsonConfig);        
        else
          this.config =  {};       
     
        //console.log(this.config)
        let config = this.config;
        if (config.propertyName === void 0) { config.propertyName = ''; }
        if (config.type === void 0) { config.type = 'Occupation'; }
        if (config.label === void 0) { config.label = 'Occupation'; }
        if (config.labelPosition === void 0) { config.labelPosition = 'Left-Left'; }
        //if (this.isPropExists(config, 'labelPosition')) { config.labelPosition = 'top'; }
        if (config.labelWidth === void 0) { config.labelWidth = 30; }
        if (config.labelMargin === void 0) { config.labelMargin = 3; }
        if (config.value === void 0) { config.value = ""; }
        if (config.case === void 0) { config.case = "none"; }
        if (config.apiUrl===void 0) {config.apiUrl ="https://occ.chooyee.co/api/query/"};
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
        "label": "Occupation Component Edit Form",
        "tabs": [
           {"tabLabel": "Display",
             "tabId":"home",
            "components": [              
              {
                "propertyName":"label",
                "label": "Label",
                "placeholder": "Occupation label",
                "description": "Enter the label for this element", 
                "validation":{"mandatory": true, "lengthCheck": {"min": 0, "max":0}},
                "value":this.config.label,           
                "type": "Textbox"
              },
              {
                "propertyName":"labelPosition",
                "label": "Label Position",
                "placeholder": "Label Position",
                "description": "Set the position of the label", 
                "options":[
                  {"Top":"Top"},
                  {"Left-Left":"Left"},
				  {"None":"None"}
                ],
                "value": this.config.labelPosition,           
                "type": "Select"
              },
              {
                "propertyName":"description",
                "label": "Descriptions (Optional)",
                "description": "Description for the element", 
                "type": "Textbox",
                "value": this.config.description
              },
              {
                "propertyName":"placeholder",
                "label": "Placeholder (Optional)",
                "placeholder": "Placeholder on the element",
                "type": "Textbox",
                "value": this.config.placeholder
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
            "tabLabel": "Validation",
            "tabId":"validate",
            "components": [
              {
                "propertyGroup":"validation",
                "propertyType":"mandatory",
                "propertyName":"validation-mandatory",
                "label": "This is mandatory field?",
                "value":this.config.validation.mandatory, 
                "type": "Checkbox"
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
              },
              {
                "propertyName":"apiUrl",
                "label": "API URL",
                "value": this.config.apiUrl, 
                "placeholder": "https://api.allianceplugin.io/occ/autocomplete",
                "description": "Set the api url for the occupation", 
                "type": "Textbox"
              }
            ]
          }
        ]
      };
    }

    renderElement = (config, elementId)=>{
		//propertyName, elementId, prefix, suffix, placeholder, letterCase, required, val
		const prefix = config.prefix;
		const suffix = config.suffix;     
		const desc = config.description;
      	//===================================================================================
        //render Input Group
        //===================================================================================
       
        const divInputGroup = this.createElement("div", {"class":"input-group"});      

		if (!this.isNullOrEmpty(prefix)){
			const divPrefix = this.createElement("div", {"ref":"prefix", "class":"input-group-text"})  //<div ref="prefix" class="input-group-text">prefix</div>
			divPrefix.innerHTML = prefix;
			divInputGroup.appendChild(divPrefix);
        }
        
        //<input aria-required="true" aria-labelledby="l-epx0fbk-textField d-epx0fbk-textField" id="epx0fbk-textField" value="" spellcheck="true" placeholder="Placeholder" autocomplete="off" lang="en" class="form-control" type="text" name="data[textField]" ref="input">
		const divFormFloat = this.createElement("div", {"class":"form-floating"}); 
		divInputGroup.appendChild(divFormFloat);

        const inputEl = this.renderRawElement(config, elementId);
        
        divFormFloat.appendChild(inputEl);
		const label = this.createElement("label",{"for":`${this.name}-${elementId}`});
		label.innerHTML = config.label;
		divFormFloat.appendChild(label);

        if (!this.isNullOrEmpty(suffix))
        {
			const divSuffix = this.createElement("div", {"ref":"suffix", "class":"input-group-text"})  //<div ref="prefix" class="input-group-text">prefix</div>
			divSuffix.innerHTML = suffix;
			divInputGroup.appendChild(divSuffix);
        }

       
        const divElement = this.createElement("div", {"ref":"element"});
        divElement.appendChild(divInputGroup);

        if (!this.isNullOrEmpty(desc))
        {
			//<div class="form-text text-muted" id="d-egxfg08-checkbox">testew er ewrewr ewrewr ewre w</div>
			const divDesc = this.createElement("div", {"class":"form-text text-muted"});
			divDesc.innerHTML = desc;
			divElement.appendChild(divDesc);
        }

        return divElement;
        //===================================================================================
        //end render Input Group
        //===================================================================================
    }

    renderRawElement = (config, elementId)=>{   
		const propertyName = config.propertyName;
		const placeholder = config.placeholder;
		const val = config.value;

		const inputEl =  this.createElement("input", {"placeholder":config.label,"id":`${this.name}-${elementId}`, "class":"form-control", "type":"text", "ref":"input"});
		
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
			inputEl.setAttribute("value",val);
		}

		if (!this.isNullOrEmpty(placeholder))
		{
			inputEl.setAttribute("placeholder", placeholder);
		}

		
		if (this.isPropExists(config,"validation"))
		{
			
			if (this.isPropExists(config.validation, "mandatory"))
			{
			inputEl.classList.add("mandatory");
			}
		}
       
		//Attach Autocmplete event
		autocomplete(inputEl,config.apiUrl) ;
      	return inputEl;
    }
  }

    //============================================================================
    //Custom Event Listener
    //===========================================================================

	let autocomplete = (inp, url) => {
		/*the autocomplete function takes two arguments,
		the text field element and an array of possible autocompleted values:*/
		let currentFocus;
		/*execute a function when someone writes in the text field:*/
		inp.addEventListener("input", function(e) {
			
		});
		
		/*execute a function presses a key on the keyboard:*/
		inp.addEventListener("keydown", function(e){
		  var x = document.getElementById(this.id + "autocomplete-list");
		  if (x) x = x.getElementsByTagName("div");
		  if (e.keyCode == 40) {
			/*If the arrow DOWN key is pressed,
			  increase the currentFocus variable:*/
			currentFocus++;
			/*and and make the current item more visible:*/
			addActive(x);
		  } else if (e.keyCode == 38) {
			//up
			/*If the arrow UP key is pressed,
			  decrease the currentFocus variable:*/
			currentFocus--;
			/*and and make the current item more visible:*/
			addActive(x);
		  } else if (e.keyCode == 13) {
			/*If the ENTER key is pressed, prevent the form from being submitted,*/
			e.preventDefault();
			if (currentFocus > -1) {
			  /*and simulate a click on the "active" item:*/
			  if (x) x[currentFocus].click();
			}

			let a, //OUTER html: variable for listed content with html-content
			b, // INNER html: filled with array-Data and html
			i, //Counter
			val = this.value;
		
			/*close any already open lists of autocompleted values*/
			closeAllLists();
		
			if (!val) {return false;}
	  
		  	currentFocus = -1;
	  
			/*create a DIV element that will contain the items (values):*/
			a = document.createElement("DIV");
			
			a.setAttribute("id", this.id + "autocomplete-list");
			a.setAttribute("class", "autocomplete-items list-group text-left");
			
			/*append the DIV element as a child of the autocomplete container:*/
			this.parentNode.appendChild(a);
		
			//const arr =  fetch(url + this.value);
			fetch(url + this.value)
			.then((response) => {
				// Check if the response status is OK (200)
				if (!response.ok) {
					console.log(response.status);
					throw new Error(`HTTP error! Status: ${response.status}`);
				}
				// Parse the response JSON		
				return response.json();
			})
			.then((arr)=>{
				//console.log(arr.length);
				for (i = 0; i < arr.length; i++) {
					/*check if the item starts with the same letters as the text field value:*/
					//if (arr[i].substr(0, val.length).toUpperCase() == val.toUpperCase()) {
						/*create a DIV element for each matching element:*/
						b = document.createElement("DIV");
						b.setAttribute("class","list-group-item list-group-item-action");
						/*make the matching letters bold:*/
						b.innerHTML = "<strong>" + arr[i].substr(0, val.length) + "</strong>";
						b.innerHTML += arr[i].substr(val.length);
						/*insert a input field that will hold the current array item's value:*/
						b.innerHTML += "<input type='hidden' value='" + arr[i] + "'>";
						/*execute a function when someone clicks on the item value (DIV element):*/
						b.addEventListener("click", function(e) {
							/*insert the value for the autocomplete text field:*/
							inp.value = this.getElementsByTagName("input")[0].value;
							/*close the list of autocompleted values,
								(or any other open lists of autocompleted values:*/
							closeAllLists();
						});
						a.appendChild(b);
						console.log(a);
					//}
				}
			})
			.catch((error) => {
				console.error('Fetch error:', error);
				throw error; // Rethrow the error to propagate it to the caller
			});
			
			/*for each item in the array...*/
			
		  }
		});
		
		let addActive = (x) => {
		  	/*a function to classify an item as "active":*/
			if (!x) return false;
			/*start by removing the "active" class on all items:*/
			removeActive(x);
			if (currentFocus >= x.length) currentFocus = 0;
			if (currentFocus < 0) currentFocus = x.length - 1;
			/*add class "autocomplete-active":*/
			x[currentFocus].classList.add("active");
		}
		
		let removeActive = (x) => {
			/*a function to remove the "active" class from all autocomplete items:*/
			for (let i = 0; i < x.length; i++) {
				x[i].classList.remove("active");
			}
		}
		
		let closeAllLists = (elmnt) => {
			/*close all autocomplete lists in the document,
			except the one passed as an argument:*/
			var x = document.getElementsByClassName("autocomplete-items");
			for (var i = 0; i < x.length; i++) {
				if (elmnt != x[i] && elmnt != inp) {
					x[i].parentNode.removeChild(x[i]);
				}
			}
		}
		
		/*execute a function when someone clicks in the document:*/
		document.addEventListener("click", function(e) {
			closeAllLists(e.target);
		});
		
	  };
