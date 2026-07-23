(function (window, document) {
  var timerFunction = function (hiddenField, siteKey) {
    window.grecaptcha.execute(siteKey, { action: 'umbracoform_submit' }).then(function (token) {
      // Enable the submit button now we have a token.
      hiddenField.form.querySelector('[type=submit]').removeAttribute('disabled');
      hiddenField.value = token;
    });
    setTimeout(timerFunction, 60 * 1000, hiddenField, siteKey);
  };

  document.addEventListener('DOMContentLoaded', function () {

    Array.from(document.getElementsByClassName("umbraco-forms-recaptcha-v3-config")).forEach(function (configElement) {

      // Get reference to hidden field.
      var hiddenField = document.getElementById(configElement.dataset.id);
      var siteKey = configElement.dataset.siteKey;

      // Disable the submit button for this form, until we actually have a key from Google reCAPTCHA.
      hiddenField.form.querySelector('[type=submit]').setAttribute('disabled', 'disabled');

      window.grecaptcha.ready(function () {
        timerFunction(hiddenField, siteKey);
      });
    });
  });
})(window, document);
