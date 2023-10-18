class Base
{    
	constructor() {        
	   this.builderEditComponents = {};
	   this.elementId =  this.getShortUUID();
	   //this.editFormJson = this.getEditFormJson();
	}

	Test = ()=>{
	  const objConfig = {}; 
	  objConfig.group = this.group;
	  objConfig.name = this.name;
	  objConfig.version = this.version;
	  objConfig.config = this.config;     
	  console.log(JSON.stringify(objConfig));
	}

	createElement(elementType, attrs={})
	{
		const el = document.createElement(elementType);
		this.appendAttr(el, attrs);
		return el;
	}

	appendAttr(el, attrs = {})
	{
		for (const [k, v] of Object.entries(attrs)) {
			el.setAttribute(k, v);
		}
		return el;
	}

	appendChildren(el, children = [])
	{
		for (const child of children) {
			el.appendChild(child);
		}
		return el;
	}

	getUUID()
	{
	  return Date.now().toString(36) + Math.random().toString(36).substr(2);
	}

	getShortUUID(length = 8) {
		let uuid = '';
		const alphabet = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
		const alphanumeric = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
		
		// Generate the first character (alphabet)
		uuid = alphabet[Math.floor(Math.random() * alphabet.length)];

		for (let i = 0; i < length; i++) {                  
		  uuid += alphanumeric[Math.floor(Math.random() * alphanumeric.length)];
		}
		
		return uuid;
	
	}

	showTooltip(element) {
		const tooltipText = element.getAttribute('data-tooltip');
		
		// Create tooltip element
		const tooltip = document.createElement('div');
		
		tooltip.classList.add('tooltip');
		tooltip.textContent = tooltipText;
	  
		// Calculate tooltip position
		const rect = element.getBoundingClientRect();
		tooltip.style.top = `${ rect.top + window.scrollY - tooltip.offsetHeight}px`;
		tooltip.style.left = `${rect.left + window.scrollX + (element.offsetWidth - tooltip.offsetWidth) / 2}px`;
		tooltip.style.position = 'absolute';
		tooltip.style.margin = '0 1rem 1rem 1rem';
		//console.log (tooltip.style.top);
		// Append tooltip to the body
		document.body.appendChild(tooltip);
	  
		//Remove the tooltip after a delay
		setTimeout(() => {
		  tooltip.remove();
		}, 2000); // Adjust the delay as needed
	}

	isPropExists =(obj, propName) =>{
	  return (obj.hasOwnProperty(propName) && obj[propName]!==null && obj[propName]!==undefined)
	}

	isNullOrEmpty(value) {
	  return value === null || value === undefined || (typeof value === 'string' && value.trim() === '') || (Array.isArray(value) && value.length === 0);
	}

	renderBuilderEditComponent = ()=>{        

	  // return `<div data-noattach="true" class="component-btn-group">
	  //   <div ref="removeComponent" class="btn btn-xxs btn-danger component-settings-button component-settings-button-remove" tabindex="-1" aria-label="Remove button. Click to remove component from the form" role="button">
	  //     <i class="fa fa-remove bi bi-trash"></i>
	  //   </div>       
	  //   <div ref="editComponent" class="btn btn-xxs btn-secondary component-settings-button component-settings-button-edit" tabindex="-1" aria-label="Edit button. Click to open component settings modal window" role="button">
	  //     <i class="fa fa-cog bi bi-gear"></i>
	  //   </div>
	  // </div>`
	  const divGroup = this.createElement("div", {"class":"component-btn-group"})
	  const divRemoveComponent = this.createElement("div",{"ref":"removeComponent", "class":"btn btn-xxs btn-danger component-settings-button component-settings-button-remove", "tabindex":"-1", "aria-label":"Remove button. Click to remove component from the form", "role":"button"});
	  divRemoveComponent.appendChild(this.createElement("i",{"class":"fa fa-remove bi bi-trash"}));

	  const divEditComponent = this.createElement("div",{"ref":"editComponent", "class":"btn btn-xxs btn-success component-settings-button component-settings-button-edit", "tabindex":"-1", "aria-label":"Edit button. Click to open component from the form", "role":"button"});
	  divEditComponent.appendChild(this.createElement("i",{"class":"fa fa-cog bi bi-sliders"}));

	  divGroup.appendChild(divRemoveComponent);
	  divGroup.appendChild(divEditComponent);

	  this.builderEditComponents["edit"] = divEditComponent;
	  this.builderEditComponents["remove"] = divRemoveComponent;
	  return divGroup;
	}

	
	getBuilderEditComponent = (btnType)=>{
	  return this.builderEditComponents[btnType];
	}
   
	trueTypeOf =  (obj)=> {
	  return Object.prototype.toString.call(obj).slice(8, -1).toLowerCase();
	};


	renderDomElement = ()=>{
		let config = this.config;

		const elementId = this.elementId;
		//Main div component
		let div = this.createElement('div', {"id":elementId,"tabindex":"-1","ref":"dragComponent", "class":"builder-component"});

		
		//Edit component
		//div.appendChild(this.getBuilderEditComponent());
		if (config.formBuilderMode){
			const dragGrouper = this.createElement("div",{"class":"drag-helper"});
			dragGrouper.appendChild(this.createElement("i",{"class":"fa fa-arrows-move bi bi-arrows-move"}));
			div.appendChild(dragGrouper);
			div.appendChild(this.renderBuilderEditComponent());
		}

		//div component
		//<div ref="component" class="mb-2 formio-form-group has-feedback formio-component formio-component-textfield formio-component-textField limau, ice" id="enuyfr7">
		let componentDiv = this.createElement('div',{"id": "component-" + elementId,"ref":"component", "class":"mb-2", "data-component-type":this.config.type});

		div.appendChild(componentDiv);
	  
	  	const mainElement = this.renderElement(config, elementId);

		//label position
		if ((!this.isNullOrEmpty(config.label)) && (config.labelPosition.toLowerCase()!=='none')){
			const label = this.renderLabel(elementId, config.label);
			
			if (this.isPropExists(config, "tooltip"))
			{
				label.appendChild(this.renderTooltip(config.tooltip));
			}
			
			//console.log(config.labelPosition)
			if (config.labelPosition.toLowerCase()=='top') 
			{       
				componentDiv.appendChild(label);
				componentDiv.appendChild(mainElement);
			}
			else if (config.labelPosition.toLowerCase()=='left-left') 
			{
				const divFieldLabel = this.renderFieldDiv(config.labelWidth, config.labelMargin);        
				divFieldLabel.appendChild(label);       
			
				const divFieldContent = this.renderFieldDiv(100 - (config.labelWidth + config.labelMargin));
				divFieldContent.appendChild(mainElement);
						
				const divFieldWrapper = this.createElement("div", {"style":"display:flex"});
				divFieldWrapper.appendChild(divFieldLabel);
				divFieldWrapper.appendChild(divFieldContent);
				componentDiv.appendChild(divFieldWrapper);
			}
		}
		else{
			console.log("in")
			console.log(mainElement)
			componentDiv.appendChild(mainElement);
		}
	  
		//==============================================================
		//Display behavior
		//===============================================================
		if (config.display==='none')
		{
			div.style.display='none';
		}
		// if (!this.isNullOrEmpty(config.displayBehavior))
		// {
		// 	eval(`${this.type}.${config.displayBehavior}()`)
		// }
	  	return div;
	}  

	renderLabel = (elementId, labelText) =>{
	  const label = this.createElement('label');
	  const labelId = `label-${elementId}`;
	  const targetElement = `${this.name}-${elementId}`;
	  const cssClass = "col-form-label";
	  this.appendAttr(label, {"id":labelId,"for":targetElement, "class":cssClass, "ref":"label"});
	  label.innerHTML = labelText;
	  return label;
	}

	renderTooltip = (tooltipText) =>{
	  //<i data-tooltip="tooltip" class="fa fa-question-sign bi bi-question-circle text-muted" tabindex="0" ref="tooltip" aria-expanded="false"></i>
	  const i = this.createElement('i');
	  const cssClass = "fa fa-question-sign bi bi-question-circle text-muted";
	  this.appendAttr(i, {"data-tooltip":tooltipText, "aria-expanded":"false", "tabindex":"0", "class":cssClass, "ref":"tooltip"});
	  i.addEventListener('mouseover', (e) => {
		this.showTooltip(i);
	  });

	  i.addEventListener('mouseleave', () => {
		this.removeTooltip(i);
	  });
	  return i;
	}

	renderFieldDiv = (labelWidth, marginPercent) =>{
	  //<div style="flex: 30; margin-right: 3%;" class="field-label"></div>
	  let style = `flex: ${labelWidth};`
	  if (marginPercent>0) {
		style +=`margin-right: ${marginPercent}%`
	  };

	  return this.createElement('div',{"style":style, "class":"field-label"});

	}

	createInstance(classNameString, json)
	{
	  var obj = new (eval(classNameString))(json);
	  return obj
	}

	//=============================================================================================
	// Edit Form
	//=============================================================================================
	renderEditForm = () =>{
		const formData = this.getEditFormJson();
        //<ul class="nav nav-pills mb-3" id="pills-tab" role="tablist">
        const ulTabHeader = this.createElement("ul", {"class":"nav nav-pills mb-3", "id":"pills-tab", "role":"tablist"})
        // <div class="tab-content" id="pills-tabContent">
        //     <div class="tab-pane fade show active" id="pills-home" role="tabpanel" aria-labelledby="pills-home-tab">...</div>
        //     <div class="tab-pane fade" id="pills-profile" role="tabpanel" aria-labelledby="pills-profile-tab">...</div>
        //     <div class="tab-pane fade" id="pills-contact" role="tabpanel" aria-labelledby="pills-contact-tab">...</div>
        //   </div>
        const divTabContentContainer = this.createElement("div", {"class":"tab-content", "id":"pills-tabContent"});
    
        let i = 0;
        formData.tabs.forEach(tab => {
            let active = i==0? true:false;

            // console.log("Tab Label:", tab.tabLabel);
            // console.log("Tab ID:", tab.tabId);
            //Render tab header
            ulTabHeader.innerHTML += this.renderTabHeader(tab.tabId, tab.tabLabel, active);

            const tabContainer = this.renderTabContentContainer(tab.tabId, active);
            // Loop through components in the tab
            tab.components.forEach(component => {
                //console.log("Type:", component.type);
                const el = this.createInstance(component.type, JSON.stringify(component));
                tabContainer.appendChild(el.renderDomElement());
            });

            divTabContentContainer.appendChild(tabContainer);
            i++;
        });
        const container = this.createElement("div", {"ref":"tabContainer"});
        container.appendChild(ulTabHeader);
        container.appendChild(divTabContentContainer);

        return container;
    }

	attachComponentEventListener = (thisEl)=>{

	}

	executeButtonSave = ()=>{
		return;
	}
	
    attachEditFormEvent = () =>{
        const formData = this.getEditFormJson();
        formData.tabs.forEach(tab => {
           
            tab.components.forEach(component => {
				if (component.hasOwnProperty("events"))
				{
					const events = component.events;
					events.forEach(ev =>{                                                  
						//console.log(`${component.type}.${ev.handler}('${ev.event}', '${ev.target}', '${component.propertyId}')`);
						eval(`${component.type}.${ev.handler}('${ev.event}', '${ev.target}', '${component.propertyId}')`);
					});
				}
               
            });
        });
      
    }
    
    renderTabHeader=(tabId, tabLabel, active)=>{
        let activeStr ="";
        if (active)
        {
            activeStr = "active";
        }
        return `<li class="nav-item" role="presentation">
                    <button class="nav-link ${activeStr}" id="pills-${tabId}-tab" data-bs-toggle="pill" data-bs-target="#pills-${tabId}" type="button" role="tab" aria-controls="pills-${tabId}" aria-selected="${active}">${tabLabel}</button>
                </li>`;
    }

    renderTabContentContainer=(tabId, active)=>{
        let activeStr ="";
        if (active)
        {
            activeStr = "show active";
        }
        //<div class="tab-pane fade show active" id="pills-home" role="tabpanel" aria-labelledby="pills-home-tab">...</div>;
        return this.createElement("div", {"class":"tab-pane fade " + activeStr, "id":`pills-${tabId}`, "role":"tabpanel", "aria-labelledby":`pills-${tabId}-tab`})
    }

	//=============================================================================================
	//Static methods
	//=============================================================================================
	static removeListeners = (targetElement, eventType)=>
	{
		const listeners = targetElement.cloneNode(true).listeners;
		
		if (listeners) {
			listeners[eventType] = [];
		}
		// Iterate through the list and remove each listener
		// if (listeners) {
		// 	listeners.forEach(listener => {
		// 		element.removeEventListener(eventType, listener.listener);
		// 	});
		// }
	}
	static trueTypeOf =  (obj)=> {
	  return Object.prototype.toString.call(obj).slice(8, -1).toLowerCase();
	};
}
