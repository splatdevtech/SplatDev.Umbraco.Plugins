(function () {


    // polyfill for matches and closest
    if (!Element.prototype.matches) Element.prototype.matches = Element.prototype.msMatchesSelector;
    if (!Element.prototype.closest) Element.prototype.closest = function (selector) {
        var el = this;
        while (el) {
            if (el.matches(selector)) {
                return el;
            }
            el = el.parentElement;
        }
    };



    //execute init() on document ready
    document.addEventListener("DOMContentLoaded", listen);

    function listen() {

        // Inline script setting umbracoFormsCollection was removed in 8.11/9.3, and replaced by config elements.
        // So if we have them, use it, otherwise fall-back to legacy method.
        var configElements = document.getElementsByClassName("umbraco-forms-form-config");
        if (configElements.length > 0) {
            var umbracoFormsCollectionFromConfig = [];
            for (var i = 0; i < configElements.length; i++) {
                var configElement = configElements[i];
                var form = {
                    formId: configElement.dataset.id,
                    pageButtonConditions: JSON.parse(configElement.dataset.serializedPageButtonConditions),
                    fieldSetConditions: JSON.parse(configElement.dataset.serializedFieldsetConditions),
                    fieldConditions: JSON.parse(configElement.dataset.serializedFieldConditions),
                    triggerConditionsCheckOn: configElement.dataset.triggerConditionsCheckOn,
                    disableValidationDependencyCheck: configElement.dataset.disableValidationDependencyCheck === "true",
                    recordValues: JSON.parse(configElement.dataset.serializedFieldsNotDisplayed),
                    formElementHtmlIdPrefix: configElement.dataset.formElementHtmlIdPrefix,
                    validationRules: configElement.dataset.serializedValidationRules ? JSON.parse(configElement.dataset.serializedValidationRules) : [],
                };
                umbracoFormsCollectionFromConfig.push(form);
            }
            initCollection(umbracoFormsCollectionFromConfig, true);  // Set the 'jsonParsed' flag, indicating that we already have a JSON structure, which we don't have with the legacy, inline script method.
        } else {
            if (typeof umbracoFormsCollection === "undefined") {
                //this will occur if this js file is loaded before the inline scripts, in which case
                //we'll listen for the inline scripts to execute a custom event.
                document.addEventListener("umbracoFormLoaded", init);
            }
            else {
                initCollection(umbracoFormsCollection);
            }
        }

        // Scrolling to the submitted form was also moved from an inline script in 8.11/9.3.
        var submittedFormElement = document.getElementById("umbraco-forms-form-submitted");
        if (submittedFormElement) {
            var formClientId = submittedFormElement.dataset.formClientId;
            scrollToSubmittedForm(formClientId);
        }
    }

    function initCollection(formsCollection, jsonParsed) {
        configureUmbracoFormsValidation(formsCollection);

        for (var i = 0; i < formsCollection.length; i++) {
            init({ form: formsCollection[i] }, jsonParsed);
        }
    }

    function init(e, jsonParsed) {

        var formItem = jsonParsed ? e.form : JSON.parse(decodeURI(e.form));

        var forms = document.querySelectorAll('.umbraco-forms-form');

        for (var i = 0; i < forms.length; i++) {
            var form = forms[i];

            if (!formItem.disableValidationDependencyCheck) {
                dependencyCheck(form);
            }

            var page = form.querySelector('.umbraco-forms-page');
            var conditions = new UmbracoFormsConditions(page,
                formItem.pageButtonConditions,
                formItem.fieldSetConditions,
                formItem.fieldConditions,
                formItem.triggerConditionsCheckOn && formItem.triggerConditionsCheckOn.length > 0 ? formItem.triggerConditionsCheckOn : "change",
                formItem.recordValues,
                formItem.formElementHtmlIdPrefix);
            conditions.watch();

            applyFormAccessibility(form);
        }
    }

    function scrollToSubmittedForm(formClientId) {
        document.onreadystatechange = function () {

            if (document.readyState == "complete") {

                // We have various cases where we want to scroll to the form rendered on the page.
                // - When we have a server-side validation error, we should scroll to the first error element (which class 'field-validation-error').
                // - When the form has been submitted, we should scroll to the displayed message (which will have class 'umbraco-forms-submitmessage' or 'umbraco-forms-submitmessage-html').
                // - When navigating back and forth on a multi-page form, we should scroll to the start of the form page (which will have an id of #umbraco_form_[form client id]).
                var validationsMessageElements = document.querySelectorAll(".field-validation-error");
                var messageOnSubmitElement = document.querySelectorAll(".umbraco-forms-submitmessage");
                var messageOnSubmitHtmlElement = document.querySelectorAll(".umbraco-forms-submitmessage-html");
                var submittedFormElement = document.getElementById("umbraco_form_" + formClientId);
                if (validationsMessageElements.length > 0) {
                    scrollToFirstFieldWithValidationError(validationsMessageElements);
                } else if (messageOnSubmitElement.length > 0) {
                    scrollElementIntoView(messageOnSubmitElement[0].parentElement);
                } else if (messageOnSubmitHtmlElement.length > 0) {
                    scrollElementIntoView(messageOnSubmitHtmlElement[0]);
                } else if (submittedFormElement) {
                    scrollElementIntoView(submittedFormElement);
                }
            }
        }

        function scrollToFirstFieldWithValidationError(validationsMessageElements) {
            var scrollY = 0;
            for (var i = 0; i < validationsMessageElements.length; i++) {
                var node = getFormFieldElement(validationsMessageElements[i]);
                var offset = node.getBoundingClientRect().top;
                if (0 < offset && (offset < scrollY || scrollY === 0)) {
                    scrollY = offset;
                }
            }

            if (scrollY > 0) {
                window.scrollTo(0, scrollY);
            }
        }

        function scrollElementIntoView(element) {
            element.scrollIntoView({
                behavior: "auto",
                block: "start",
            });
        }

        function getFormFieldElement(node) {
            var runner = node;
            while (runner.tagName !== "BODY") {
                if (runner.classList.contains("umbraco-forms-field")) {
                    return runner;
                }

                runner = runner.parentNode;
            }

            return node;
        }
    }

    /** Configures the jquery validation for Umbraco forms */
    function configureUmbracoFormsValidation(formsCollection) {

        var submitInputs, input;

        if (window.aspnetValidation !== undefined) {
            // Asp-net validation setup:

            var validationService = new aspnetValidation.ValidationService();
            var required = function (value, element) {
                // Handle single and multiple checkboxes:
                if (element.type.toLowerCase() === "checkbox" || element.type.toLowerCase() === "radio") {
                    var allCheckboxesOfThisName = element.form.querySelectorAll("input[name='" + element.name + "']");
                    for (var i = 0; i < allCheckboxesOfThisName.length; i++) {
                        if (allCheckboxesOfThisName[i].checked === true) {
                            return true;
                        }
                    }
                    return false;
                }
                return Boolean(value);
            }
            validationService.addProvider("requiredcb", required);
            validationService.addProvider("required", required);// this will go instead of the build-in required.

            var umbracoforms_regex = function (value, element, params) {
                if (!value || !params.pattern) {
                    return true;
                }

                var r = new RegExp(params.pattern);
                return r.test(value);
            }
            validationService.addProvider("umbracoforms_regex", umbracoforms_regex);

            var wrapProviderWithIgnorerBehaviour = function (provider) {
                return function (value, element, params) {

                    // If field is hidden we ignore the validation.
                    if (element.offsetParent === null) {
                        return true;
                    }

                    return provider(value, element, params);
                }
            }

            // we can only incept with default validator if we do it after bootstrapping but before window load event triggers validationservice.
            window.addEventListener('load', function () {

                // Wrap all providers with ignorer hidden fields logic:
                for (var key in validationService.providers) {
                    validationService.providers[key] = wrapProviderWithIgnorerBehaviour(validationService.providers[key]);
                }
            });

            // bootstrap validation service.
            validationService.bootstrap();

            // make validation service availalble globally for custom client-side code.
            window.umbracoFormsValidationService = validationService;

            // Without jquery validation, the previous page submit button click isn't sent with it's name,
            // so can't be used server-side to determine whether to go forward or back.
            // Hence we'll use an alternate method, setting a hidden field that's also used in the check.
            var handlePreviousClicked = function () {
                this.form.elements["PreviousClicked"].value = "clicked";
            };

            // - previous buttons are indicated by a data attribute of data-umb="prev-forms-form", introduced in 8.12/9.4.
            var previousButtonElements = document.querySelectorAll("[data-umb='prev-forms-form']");

            // - prior to that classes were used to identify them, so for compatibility with custom themes, we'll check for those too
            //   if we haven't found any via the data attribute.
            if (previousButtonElements.length === 0) {
                previousButtonElements = document.getElementsByClassName("prev cancel");
            }

            for (var i = 0; i < previousButtonElements.length; i++) {
                previousButtonElements[i].form.elements["PreviousClicked"].value = "";
                previousButtonElements[i].addEventListener('click', handlePreviousClicked, false);
            }

            // Apply protection for multiple form submissions using the aspnet validation validator.
            submitInputs = getSubmitInputs();
            for (let i = 0; i < submitInputs.length; i++) {
                input = submitInputs[i];
                input.addEventListener("click", function (e) {
                    e.preventDefault();
                    var frm = e.currentTarget.form;
                    var validationRules = getValidationRulesForForm(frm, formsCollection);
                    validationService.validateForm(frm, (valid) => {
                        var formRulesValid = validateFormRules(frm, validationRules, "aspnet-client-validation", validationService);
                        valid = valid && formRulesValid;
                        if (valid) {
                            submitValidatedForm(frm);
                            var submitInputsForForm = frm.querySelectorAll("[type=submit]:not(.cancel)");
                            for (let j = 0; j < submitInputsForForm.length; j++) {
                                submitInputsForForm[j].setAttribute("disabled", "disabled");
                            }
                        }
                    });
                });
            }
        } else if (typeof jQuery === "function" && $.validator && $.validator.unobtrusive) {
            //Jquery validation setup

            $.validator.setDefaults({
                ignore: ":hidden"
            });

            $.validator.unobtrusive.adapters.addBool("requiredcb", "required");

            $.validator.addMethod("umbracoforms_regex", function (value, element) {

                var regex = $(element).attr("data-regex");
                var val = $(element).val();
                if (val.length === 0) {
                    return true;
                }
                return val.match(regex);
            });

            $.validator.unobtrusive.adapters.addBool("regex", "umbracoforms_regex");

            // Apply protection for multiple form submissions using the jquery validator.
            submitInputs = getSubmitInputs();
            for (let i = 0; i < submitInputs.length; i++) {
                input = submitInputs[i];
                input.addEventListener("click", function (evt) {
                    evt.preventDefault();
                    var frm = $(this).closest("form");
                    resetValidationMessages(frm[0]);
                    var validator = frm.validate();
                    var validationRules = getValidationRulesForForm(frm[0], formsCollection);
                    var formValid = frm.valid();
                    var formRulesValid = validateFormRules(frm[0], validationRules, "jquery", validator);
                    if (formValid && formRulesValid) {
                        submitValidatedForm(frm);
                        this.setAttribute("disabled", "disabled");
                    }
                }.bind(input));
            }
        }
    }

    function getSubmitInputs() {
        return document.querySelectorAll(".umbraco-forms-form [type=submit]:not(.cancel)");
    }

    function getValidationRulesForForm(formElement, formsCollection) {
        var filteredForms = formsCollection.filter(f => f.formId === formElement["FormId"].value.replaceAll("-", ""));
        if (filteredForms.length > 0 && filteredForms[0].validationRules) {

            // Work on a clone of the rules, so we don't modify in the originals and can still access the definitions that refer to
            // field names.
            var clonedRules = structuredClone(filteredForms[0].validationRules);
            var formData = new FormData(formElement);
            return clonedRules.map(r => getRuleWithFieldValues(r, formData));
        }

        return [];
    }

    function getRuleWithFieldValues(validationRule, formData) {
        var replacedRule = validationRule.rule;
        for (var item of formData) {
            replacedRule = replacedRule.replaceAll("{" + item[0] + "}", item[1]);
        }
        validationRule.rule = replacedRule;
        return validationRule;
    }

    function validateFormRules(formElement, validationRules, validationLibrary, validator) {
        if (typeof jsonLogic === 'undefined') {
            // If we don't have the dependency, form rules aren't validated.
            return true;
        }

        var rulesResult = true;
        for (var i = 0; i < validationRules.length; i++) {
            var validationRule = validationRules[i];

            // Skip if the rule contains unresolved field references.
            var re = /{({*[\w]*}*)}/;
            if (validationRule.rule.match(re)) {
                continue;
            }

            var ruleAsJson;
            try {
                ruleAsJson = JSON.parse(validationRule.rule);
            } catch (e) {
                continue; // Skip also on invalid JSON.
            }

            var ruleResult = jsonLogic.apply(ruleAsJson);
            indicateFormRuleValidation(formElement, validationLibrary, validator, ruleResult, validationRule);
            rulesResult = rulesResult && ruleResult;
        }

        return rulesResult;
    }

    function indicateFormRuleValidation(formElement, validationLibrary, validator, result, validationRule) {
        if (validationRule.fieldId) {
            // Show or hide field error dependending on result.
            var inputElement = formElement[validationRule.fieldId];

            // If we have more than one element, e.g. for the date picker, use the one also matching the id.
            if (inputElement.length && inputElement.length > 0) {
                inputElement = Array.from(inputElement).filter(e => e.id === validationRule.fieldId)[0];
            }

            if (validationLibrary === "aspnet-client-validation") {
                if (result) {
                    // Only remove if it's the same message that we've added (could be other validation rules on the field we would want to keep).
                    var validationFieldMessageElement = getValidationFieldMessageElement(inputElement, "error");
                    if (validationFieldMessageElement && validationFieldMessageElement.innerText === validationRule.errorMessage) {
                        validator.removeError(inputElement);
                    }
                } else {
                    validator.addError(inputElement, validationRule.errorMessage);
                }
            } else if (validationLibrary === "jquery") {
                var validationSummaryElement = formElement.querySelector(".validation-summary-errors ul");
                if (result) {
                    var validationFieldMessageElement = getValidationFieldMessageElement(inputElement, "error");

                    // Only remove if it's the same message that we've added (could be other validation rules on the field we would want to keep).
                    if (validationFieldMessageElement && validationFieldMessageElement.innerText === validationRule.errorMessage) {
                        validationFieldMessageElement.classList.remove("field-validation-error");
                        validationFieldMessageElement.classList.add("field-validation-valid");
                        validationFieldMessageElement.innerText = "";
                    }

                    removeValidationSummaryMessage(validationSummaryElement, validationRule.errorMessage);
                } else {
                    var validationFieldMessageElement = getValidationFieldMessageElement(inputElement, "valid");
                    if (validationFieldMessageElement) {
                        validationFieldMessageElement.classList.remove("field-validation-valid");
                        validationFieldMessageElement.classList.add("field-validation-error");
                        validationFieldMessageElement.innerText = validationRule.errorMessage;
                    }

                    addValidationSummaryMessage(validationSummaryElement, validationRule.errorMessage);
                }
            }
        }
    }

    function getValidationFieldMessageElement(inputElement, state) {
        return inputElement.closest(".umbraco-forms-field-wrapper").querySelector(".field-validation-" + state);
    }

    function removeValidationSummaryMessage(validationSummaryElement, errorMessage) {
        if (validationSummaryElement) {
            validationSummaryElement.querySelectorAll("li").forEach(li => {
                if (li.innerText === errorMessage) {
                    li.remove();
                }
            });
        }
    }

    function addValidationSummaryMessage(validationSummaryElement, errorMessage) {
        removeValidationSummaryMessage(validationSummaryElement, errorMessage);
        if (validationSummaryElement) {
            var validationSummaryItemElement = document.createElement("li");
            validationSummaryItemElement.appendChild(document.createTextNode(validationRule.errorMessage));
            validationSummaryElement.appendChild(validationSummaryItemElement);
        }
    }

    function submitValidatedForm(form) {
        // Use requestSubmit if available (see: https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/requestSubmit#usage_notes)
        if (form.requestSubmit) {
            form.requestSubmit();
        } else {
            form.submit();
        }
    }

    /**
     * method to determine if Umbraco Forms can run and has the required dependencies loaded
     * @param {Form Element} formEl the element of the form
     */
    function dependencyCheck(formEl) {
        // Only perform check if the global 'Umbraco.Sys' is null/undefined.
        // If present means we are in backoffice & that this is being rendered as a macro preview and we do not need to perform this check here.
        // Similarly we need a check for if running in a rich text editor.
        var isBackOffice = function () {
            return typeof Umbraco !== "undefined" && typeof Umbraco.Sys !== "undefined";
        };
        var isBackOfficeRte = function () {
            return document.body.id === "tinymce";
        };

        if (isBackOffice() || isBackOfficeRte()) {
            return;
        }

        var errorElement = document.createElement("div");
        errorElement.className = "umbraco-forms missing-library";
        errorElement.style.color = "#fff";
        errorElement.style.backgroundColor = "#9d261d";
        errorElement.style.padding = "15px";
        errorElement.style.margin = "10px 0";
        var errorMessage = "";

        //Ensure umbracoForm is not null
        if (formEl) {

            //Check to see if the message for the form has been inserted already
            var checkForExistinhgErr = formEl.getElementsByClassName('umbraco-forms missing-library');
            if (checkForExistinhgErr.length > 0) {
                return;
            }

            var hasValidationFramework = false;

            if (window.jQuery && $ && $.validator !== undefined) {
                hasValidationFramework = true;
            } else if (window.aspnetValidation !== undefined) {
                hasValidationFramework = true;
            }

            if (hasValidationFramework === false) {
                errorMessage = errorMessage + "Umbraco Forms requires a validation framework to run, please read documentation for posible options.";
            }

            if (errorMessage !== "") {
                errorElement.innerHTML = errorMessage + '<br/> <a href="https://docs.umbraco.com/umbraco-forms/developer/prepping-frontend" target="_blank" style="text-decoration:underline; color:#fff;">See Umbraco Forms Documentation</a>';

                formEl.insertBefore(errorElement, formEl.childNodes[0]);
            }
        }
    }

    /**
     * Applies form accessibility improvements.
     * @param {Element} formEl the element of the form.
     */
    function applyFormAccessibility(formEl) {
        setFocusToFirstElementOnValidationError(formEl);
    }

    /**
     * Monitors for validation errors and when found sets the focus to the first field with an error.
     * @param {Element} formEl the element of the form.
     */
    function setFocusToFirstElementOnValidationError(formEl) {
        if ("MutationObserver" in window === false) {
            return;
        }

        // To implement this, we are relying on on monitoring for validation message elements, which only fires when there are changes.
        // So if you have two errors, and fix the first one, it wouldn't then highlight the second one on re-submitting the form.
        // Unless we reset the validation messages on submit, so they get changed back on errors.
        if (window.aspnetValidation !== undefined) {
            var form = formEl.getElementsByTagName('form')[0];
            var handleResetValidationMessages = function () {
                resetValidationMessages(form);
            };
            form.addEventListener('submit', handleResetValidationMessages, false);
        } else {
            // For jquery.validate, we need to hook this in as part of the submit handler coded in configureUmbracoFormsValidation();
        }

        // Watch for changes to the validation error messages in the DOM tree using a MutationObserver.
        // See: https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver
        var observer = new MutationObserver(function (mutationRecords) {
            for (var i = 0; i < mutationRecords.length; i++) {
                var mutationRecord = mutationRecords[i];
                if (mutationRecord.target.className === 'field-validation-error') {
                    setFocusOnFormField(mutationRecord.target);
                    break;
                }
            }
        });

        observer.observe(formEl, {
            attributes: true,
            attributeFilter: ['class'],
            childList: false,
            characterData: false,
            subtree: true
        });
    }

    /**
     * Resets the validation messages for a form.
     * @param {Element} formEl the element of the form.
     */
    function resetValidationMessages(formEl) {
        var validationErrorMessageElements = formEl.getElementsByClassName('field-validation-error');
        for (var i = 0; i < validationErrorMessageElements.length; i++) {
            validationErrorMessageElements[i].className = 'field-validation-valid';
        }
    }

    /**
     * Sets the focus to the form field input element associated with the provided validation message element.
     * @param {Element} validationErrorEl the element of the validation error.
     */
    function setFocusOnFormField(validationErrorEl) {
        var formFieldElement = validationErrorEl.previousElementSibling;
        while (formFieldElement) {
            if (formFieldElement.tagName.toLowerCase() === 'input' ||
                formFieldElement.tagName.toLowerCase() === 'textarea' ||
                formFieldElement.tagName.toLowerCase() === 'select') {
                formFieldElement.focus();
                break;
            }

            if (formFieldElement.classList.contains("radiobuttonlist") ||
                formFieldElement.classList.contains("checkboxlist")) {
                for (var i = 0; i < formFieldElement.children.length; i++) {
                    var formFieldChildElement = formFieldElement.children[i];
                    if (formFieldChildElement.tagName.toLowerCase() === 'input') {
                        formFieldChildElement.focus();
                        break;
                    }
                }

                break;
            }

            formFieldElement = formFieldElement.previousElementSibling;
        }
    }

    /**
     * Class to handle Umbraco Forms conditional statements
     * @param {any} form a reference to the form
     * @param {any} pageButtonConditions a reference to the page button conditions
     * @param {any} fieldsetConditions a reference to the fieldset conditions
     * @param {any} fieldConditions a reference to the field conditions
     * @param {any} triggerConditionsCheckOn the client-side event on which condition checks should be triggered
     * @param {any} values the form values
     * @param {any} formElementHtmlIdPrefix the HTML ID prefix used when rendering the form
     * @return {Object} reference to the created class
     */
    function UmbracoFormsConditions(form, pageButtonConditions, fieldsetConditions, fieldConditions, triggerConditionsCheckOn, values, formElementHtmlIdPrefix) {

        //our conditions "class" - must always be newed to work as it uses a form instance to operate on
        //load all the information from the dom and serverside info and then the class will take care of the rest

        var self = {};
        self.form = form;
        self.pageButtonConditions = pageButtonConditions;
        self.fieldsetConditions = fieldsetConditions;
        self.fieldConditions = fieldConditions;
        self.triggerConditionsCheckOn = triggerConditionsCheckOn;
        self.values = values;
        self.formElementHtmlIdPrefix = formElementHtmlIdPrefix;
        self.dataTypes = {};

        //Iterates through all the form elements found on the page to update the registered value
        function populateFieldValues(page, formValues, dataTypes) {

            var selectFields = page.querySelectorAll("select");
            for (let i = 0; i < selectFields.length; i++) {
                const field = selectFields[i];
                formValues[field.getAttribute("id")] = field.value;
                dataTypes[field.getAttribute("id")] = "select";
            }

            var textareaFields = page.querySelectorAll("textarea");
            for (let i = 0; i < textareaFields.length; i++) {
                const field = textareaFields[i];
                formValues[field.getAttribute("id")] = field.value;
                dataTypes[field.getAttribute("id")] = "textarea";
            }

            var getKeyForRadioOrCheckBoxListElement = function (field) {
                const idParts = field.getAttribute("id").split("_");
                return idParts.slice(0, idParts.length - 1).join("_");
            }

            // clear out all saved checkbox values to we can safely append
            var checkboxFields = page.querySelectorAll("input[type=checkbox]");
            for (let i = 0; i < checkboxFields.length; i++) {
                const field = checkboxFields[i];

                // Need to handle both a single checkbox, and a checkbox list. We can distinguish by comparing the field's
                // id and name attributes. If they are the same, accounting for a custom prefix, then we have a single checkbox.
                if (field.getAttribute("id") !== self.formElementHtmlIdPrefix + field.getAttribute("name")) {
                    // Handle checkbox list.
                    // For a checkbox in a list we need to remove the suffix from the Id (which will be '_0', '_1' etc.).
                    const checkboxKey = getKeyForRadioOrCheckBoxListElement(field);
                    formValues[checkboxKey] = null;
                    dataTypes[checkboxKey] = "checkbox";
                } else {
                    // Handle single checkbox.
                    formValues[field.getAttribute("id")] = null;
                    dataTypes[field.getAttribute("id")] = "checkbox";
                }
            }

            var inputFields = page.querySelectorAll("input");
            for (let i = 0; i < inputFields.length; i++) {
                const field = inputFields[i];

                if (field.getAttribute('type') === "text" ||
                    field.getAttribute('type') === "number" ||
                    field.getAttribute('type') === "email" ||
                    field.getAttribute('type') === "url" ||
                    field.getAttribute('type') === "tel" ||
                    field.getAttribute('type') === "time" ||
                    field.getAttribute('type') === "date" ||
                    field.getAttribute('type') === "datetime-local" ||
                    field.getAttribute("type") === "hidden") {
                    formValues[field.getAttribute("id")] = field.value;
                    dataTypes[field.getAttribute("id")] = "text";
                }

                if (field.getAttribute('type') === "radio") {
                    if (field.matches(':checked')) {
                        // For a radio button in a list we need to remove the suffix from the Id (which will be '_0', '_1' etc.).
                        const radioKey = getKeyForRadioOrCheckBoxListElement(field);
                        formValues[radioKey] = field.value;
                        dataTypes[radioKey] = "radio";
                    }
                }

                if (field.getAttribute("type") === "checkbox") {
                    // Again, need to handle both a single checkbox, and a checkbox list.
                    if (field.getAttribute("id") !== self.formElementHtmlIdPrefix + field.getAttribute("name")) {
                        // Handle checkbox list.
                        // For a checkbox in a list we need to remove the suffix from the Id (which will be '_0', '_1' etc.).
                        const checkboxKey = getKeyForRadioOrCheckBoxListElement(field);
                        if (field.matches(":checked")) {
                            if (formValues[checkboxKey] === null) {
                                formValues[checkboxKey] = field.value;
                            }
                            else {
                                formValues[checkboxKey] += ";;" + field.value;
                            }
                        }
                    }
                    else {
                        // Handle single checkbox.
                        formValues[field.getAttribute("id")] = (field.matches(":checked") ? "true" : "false");
                    }
                }
            }
        }

        /* Public api */

        self.operators = {
            Is: function (value, expected) {
                if ((value || "") === expected) {
                    return true;
                }
                if (value == null) {
                    return (expected == value);
                }

                // Handle special cases where we case insensitively compare for expected values of "true" and "on", which we
                // expect to have come from checkbox conditions.
                if (expected.toUpperCase() === "TRUE" || expected.toUpperCase() === "ON") {
                    expected = "true"
                } else if (expected.toUpperCase() === "FALSE" || expected.toUpperCase() === "OFF") {
                    expected = "false"
                }

                // Handle single checked checkbox, where we'll get two values provided.
                if (value === "true;;false") {
                    value = "true";
                }

                var values = value.split(';;');

                var matchingExpected = values.filter(
                    function (o) {
                        return o === expected;
                    });
                return matchingExpected.length === values.length;  // If there are multiple values (e.g. for a checkbox list), "is" only matches if
                                                                   // a single value only matches the expected. Contains should be used if matching any value.
            },
            IsNot: function (value, unexpected, dataType) {
                if (value == null) {
                    return (unexpected != value);
                }

                // Handle single checked checkbox, where we'll get two values provided.
                if (value === "true;;false") {
                    value = "true";
                }

                var values = value.split(';;');
                var matchingUnexpected = values.filter(
                    function (o) {
                        return o === unexpected;
                    });

                if (dataType === "checkbox") {
                    if (unexpected.toUpperCase() === "TRUE" || unexpected.toUpperCase() === "ON") {
                        unexpected = "true"
                    } else if (unexpected.toUpperCase() === "FALSE" || unexpected.toUpperCase() === "OFF") {
                        unexpected = "false"
                    }
                }
                return (value || "") !== unexpected && matchingUnexpected.length != values.length;
            },
            GreaterThen: function (value, limit) {
                if (isNaN(value) || isNaN(limit)) {
                    return value > limit;
                }

                return parseInt(value) > parseInt(limit);
            },
            LessThen: function (value, limit) {
                if (isNaN(value) || isNaN(limit)) {
                    return value < limit;
                }

                return parseInt(value) < parseInt(limit);
            },
            StartsWith: function (value, criteria) {
                return value && value.startsWith(criteria);
            },
            EndsWith: function (value, criteria) {
                return value && value.endsWith(criteria);
            },
            Contains: function (value, criteria) {
                return value && value.indexOf(criteria) > -1;
            },
            ContainsIgnoreCase: function (value, criteria) {
                return value && criteria && value.toUpperCase().indexOf(criteria.toUpperCase()) >= 0;
            },
            StartsWithIgnoreCase: function (value, criteria) {
                return value && criteria && value.toUpperCase().startsWith(criteria.toUpperCase());
            },
            EndsWithIgnoreCase: function (value, criteria) {
                return value && criteria && value.toUpperCase().endsWith(criteria.toUpperCase());
            },
            NotContains: function (value, criteria) {
                return !value || (criteria && value.indexOf(criteria) < 0);
            },
            NotStartsWith: function (value, criteria) {
                return !value || (criteria && !value.startsWith(criteria));
            },
            NotEndsWith: function (value, criteria) {
                return !value || (criteria && !value.endsWith(criteria));
            },
            NotContainsIgnoreCase: function (value, criteria) {
                return !value || (criteria && value.toUpperCase().indexOf(criteria.toUpperCase()) < 0);
            },
            NotStartsWithIgnoreCase: function (value, criteria) {
                return !value || (criteria && !value.toUpperCase().startsWith(criteria.toUpperCase()));
            },
            NotEndsWithIgnoreCase: function (value, criteria) {
                return !value || (criteria && !value.toUpperCase().endsWith(criteria.toUpperCase()));
            },
        };

        self.watch = function () {
            // This is a special case for pikaday
            // The only way around to pickup the value, for now, is to
            // subscribe to blur events
            var datepickerfields = self.form.querySelectorAll('.datepickerfield');
            for (let i = 0; i < datepickerfields.length; i++) {
                const field = datepickerfields[i];
                field.addEventListener('blur', function () {
                    if (this.value === "") {
                        // Here comes the hack
                        // Force the hidden datepicker field the datepicker field
                        var id = this.getAttribute("id");
                        var hiddenDatePickerField = id + "_1";
                        self.values[hiddenDatePickerField] = "";
                        document.getElementById(hiddenDatePickerField).value = "";// sadly we cant use querySelector with current mark-up (would need to prefix IDs)
                    }

                    populateFieldValues(self.form, self.values, self.dataTypes);
                    //process the conditions
                    self.run();
                }.bind(field));
            }
            //subscribe to change or input event as per configuration
            var changeablefields = self.form.querySelectorAll("input, textarea, select");
            for (let i = 0; i < changeablefields.length; i++) {
                const field = changeablefields[i];
                field.addEventListener(self.triggerConditionsCheckOn, function () {
                    populateFieldValues(self.form, self.values, self.dataTypes);
                    //process the conditions
                    self.run();
                }.bind(field));
            }

            //register all values from the current fields on the page
            populateFieldValues(self.form, self.values, self.dataTypes);

            //the initial run-through of all the conditions
            self.run();
        };

        self.run = function () {
            var pageId,
                htmlFieldId,
                fsId,
                fieldId,

                /*
                fsConditions = params.fsConditions || {},
                fieldConditions = params.fieldConditions || {},
                values = params.values || {},*/

                cachedResults = {};

            function evaluateRuleInstance(rule) {
                var value = self.values[self.formElementHtmlIdPrefix + rule.field],
                    func = self.operators[rule.operator],
                    result = value !== null && func(value, rule.value);
                return result;
            }

            function evaluateRule(rule) {
                var dependencyIsVisible = true;

                if (self.fieldConditions[rule.field]) {
                    dependencyIsVisible = isVisible(rule.field, self.fieldConditions[rule.field]);
                }

                if (dependencyIsVisible) {
                    return evaluateRuleInstance(rule);
                }
                else {
                    return false;
                }
            }

            function evaluateCondition(id, condition) {
                // This was once pretty. Now it needs refactoring again. :)

                var any = condition.logicType === "Any",
                    all = condition.logicType === "All",
                    fieldsetVisibilities = {},
                    hasHiddenFieldset = false,
                    success = true,
                    rule,
                    i;

                // If we don't have any rules defined, we must return false (as neither 'any' nor 'all' of the conditions
                // can be considered passing).
                if (condition.rules.length === 0) {
                    return false;
                }

                for (i = 0; i < condition.rules.length; i++) {
                    rule = condition.rules[i];

                    if (id === rule.field || id === rule.fieldsetId) {
                        throw new Error("Field or fieldset " + id + " has a condition on itself.");
                    }

                    if (fieldsetVisibilities[rule.fieldsetId] !== undefined) {
                        continue;
                    }

                    if (self.fieldsetConditions[rule.fieldsetId]) {

                        fieldsetVisibilities[rule.fieldsetId] =
                            isVisible(rule.fieldsetId, self.fieldsetConditions[rule.fieldsetId]);

                        if (!fieldsetVisibilities[rule.fieldsetId]) {
                            hasHiddenFieldset = true;
                        }
                    }
                    else {
                        fieldsetVisibilities[rule.fieldsetId] = true;
                    }
                }

                if (all && hasHiddenFieldset) {
                    return false;
                }

                for (i = 0; i < condition.rules.length; i++) {
                    rule = condition.rules[i];

                    if (fieldsetVisibilities[rule.fieldsetId]) {
                        success = evaluateRule(condition.rules[i]);
                    }
                    else {
                        success = false;
                    }

                    if (any && success) {
                        break;
                    }
                    if (all && !success) {
                        break;
                    }
                }
                return success;
            }

            function evaluateConditionVisibility(id, condition) {
                var show = condition.actionType === "Show",
                    cachedResult = cachedResults[id];

                var success;
                if (cachedResult === undefined) {
                    cachedResults[id] = show; // set default value to avoid circular issues
                    success = (cachedResults[id] = evaluateCondition(id, condition));
                } else {
                    success = cachedResult;
                }

                var visible = !(success ^ show);
                return visible;
            }

            function isVisible(id, condition) {
                if (condition) {
                    return evaluateConditionVisibility(id, condition);
                }
                return true;
            }

            function handleCondition(element, id, condition) {
                var shouldShow = isVisible(id, condition);
                if (element) {
                    if (shouldShow) {
                        element.classList.remove("umbraco-forms-hidden");
                    }
                    else {
                        element.classList.add("umbraco-forms-hidden");
                    }
                }
            }

            for (pageId in self.pageButtonConditions) {
                if (Object.prototype.hasOwnProperty.call(self.pageButtonConditions, pageId)) {
                    var pageElem = document.getElementById(pageId);
                    if (pageElem) {
                        handleCondition(pageElem.querySelector("input[name='__next'], button[name='__next'], input[data-form-navigate='next'], button[data-form-navigate='next']"), pageId, self.pageButtonConditions[pageId], "Page");
                    }
                }
            }

            for (fsId in self.fieldsetConditions) {
                if (Object.prototype.hasOwnProperty.call(self.fieldsetConditions, fsId)) {
                    htmlFieldId = self.formElementHtmlIdPrefix + fsId;
                    handleCondition(document.getElementById(htmlFieldId), fsId, self.fieldsetConditions[fsId], "Fieldset");// sadly we cant use querySelector with current mark-up (would need to prefix IDs)
                }
            }

            for (fieldId in self.fieldConditions) {
                if (Object.prototype.hasOwnProperty.call(self.fieldConditions, fieldId)) {
                    htmlFieldId = self.formElementHtmlIdPrefix + fieldId;
                    if (document.getElementById(htmlFieldId)) {
                        handleCondition(document.getElementById(htmlFieldId).closest(".umbraco-forms-field"),// sadly we cant use querySelector with current mark-up (would need to prefix IDs)
                            fieldId,
                            self.fieldConditions[fieldId],
                            "Field");
                    }
                }
            }
        };

        return self;
    }
})();
