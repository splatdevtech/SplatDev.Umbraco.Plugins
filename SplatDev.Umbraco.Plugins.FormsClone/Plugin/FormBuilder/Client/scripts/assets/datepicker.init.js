(function() {

    //execute init() on document ready
    if (document.readyState === "complete" || (document.readyState !== "loading" && !document.documentElement.doScroll)) {
        listen();
    } else {
        document.addEventListener("DOMContentLoaded", listen);
    }

    function listen() {

        // Inline script setting FormBuildersLocale was removed in 8.11/9.3, and replaced by a config element.
        // So if we have that, use it, otherwise fall-back to legacy method.
        var configElement = document.getElementById("umbraco-forms-date-picker-config");
        if (configElement) {
            var FormBuildersLocaleFromConfig = {
                name: configElement.dataset.name,
                datePickerYearRange: configElement.dataset.yearRange,
                locales: {
                    previousMonth: configElement.dataset.previousMonth,
                    nextMonth: configElement.dataset.nextMonth,
                    months: configElement.dataset.months.split(','),
                    weekdays: configElement.dataset.weekdays.split(','),
                    weekdaysShort: configElement.dataset.weekdaysShort.split(',')
                },
                format: configElement.dataset.format ?? "LL"
            };
            init({ FormBuildersLocale: FormBuildersLocaleFromConfig });
        } else {
            if (typeof FormBuildersLocale === "undefined") {
                //this will occur if this js file is loaded before the inline scripts, in which case
                //we'll listen for the inline scripts to execute a custom event.
                document.addEventListener("FormBuildersLocaleLoaded", init);
            }
            else {
                init({ FormBuildersLocale: FormBuildersLocale });
            }
        }
    }

    function init(e) {

        if (typeof moment === "undefined") {
            throw "moment lib has not been loaded";
        }

        moment.locale(e.FormBuildersLocale.name);

        var datePickerFields = document.getElementsByClassName('datepickerfield');
        for (var i = 0; i < datePickerFields.length; i++) {
            var field = datePickerFields[i];

            var options = {
                field: field,
                yearRange: e.FormBuildersLocale.datePickerYearRange,
                //ariaLabel: ariaLabel,
                i18n: e.FormBuildersLocale.locales,
                format: e.FormBuildersLocale.format,
                onSelect: function (date) {
                    setShadow(this, date);
                    var evt = document.createEvent("HTMLEvents");
                    evt.initEvent("input", false, true);
                    this._o.field.dispatchEvent(evt);
                },
                minDate: new Date('1753-01-01T00:00:00'), //Min value of datetime in SQL Server CE
                defaultDate: new Date(field.value),
                setDefaultDate: true
            };

            // If we've set an aria-label on the field already, use it so we don't have it overwritten by the Pikaday default.
            var ariaLabel = field.getAttribute("aria-label");
            if (ariaLabel) {
                options["ariaLabel"] = ariaLabel;
            }

            new Pikaday(options);
        }

        function setShadow(pickaday, date) {
            var id = pickaday._o.field.id + "_1";
            var value = moment(date).format('YYYY-MM-DD');
            var field = document.getElementById(id);
            field.value = value;
        }
    }

})();

