function check_uncheck_checkbox(isChecked) {
	if (isChecked) {
		$('input[name="userId"]').each(function () {
			this.checked = true;
		});
	} else {
		$('input[name="userId"]').each(function () {
			this.checked = false;
		});
	}
}