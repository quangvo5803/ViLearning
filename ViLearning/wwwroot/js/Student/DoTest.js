// Clock
$step = 1;
$loops = Math.round(100 / $step);
$increment = 360 / $loops;
$half = Math.round($loops / 2);
$barColor = '#28a745';
$backColor = '#c8fff1';

$(function () {
	clock.init();
});
clock = {
	interval: null,
	init: function () {
		clock.stop();
		clock.start(parseInt($('.test-duration').text(), 10));
	},
	start: function (t) {
		var pie = 0;
		var num = 0;
		var min = t ? t : 1;
		var sec = min * 60;
		var lop = sec;
		$('.count').text(min);
		if (min > 0) {
			$('.count').addClass('min')
		} else {
			$('.count').addClass('sec')
		}
		clock.interval = setInterval(function () {
			sec = sec - 1;
			if (min > 1) {
				pie = pie + (100 / (lop / min));
			} else {
				pie = pie + (100 / (lop));
			}
			if (pie >= 101) { pie = 1; }
			num = (sec / 60).toFixed(2).slice(0, -3);
			if (num == 0) {
				$('.count').removeClass('min').addClass('sec').text(sec);
			} else {
				$('.count').removeClass('sec').addClass('min').text(num);
			}
			//$('.clock').attr('class','clock pro-'+pie.toFixed(2).slice(0,-3));
			//console.log(pie+'__'+sec);
			$i = (pie.toFixed(2).slice(0, -3)) - 1;
			if ($i < $half) {
				$nextdeg = (90 + ($increment * $i)) + 'deg';
				$('.clock').css({ 'background-image': 'linear-gradient(90deg,' + $backColor + ' 50%,transparent 50%,transparent),linear-gradient(' + $nextdeg + ',' + $barColor + ' 50%,' + $backColor + ' 50%,' + $backColor + ')' });
			} else {
				$nextdeg = (-90 + ($increment * ($i - $half))) + 'deg';
				$('.clock').css({ 'background-image': 'linear-gradient(' + $nextdeg + ',' + $barColor + ' 50%,transparent 50%,transparent),linear-gradient(270deg,' + $barColor + ' 50%,' + $backColor + ' 50%,' + $backColor + ')' });
			}
			if (sec == 0) {
				clearInterval(clock.interval);
				$('.count').text(0);
				//$('.clock').removeAttr('class','clock pro-100');
				$('.clock').removeAttr('style');
				$('.clock').removeClass('clock pro-100'); // Remove the specific class
				$('#TestForm').submit(); // Submit the form automatically
			}
		}, 1000);
	},
	stop: function () {
		clearInterval(clock.interval);
		$('.count').text(0);
		$('.clock').removeAttr('style');
	}
} // End clock

document.getElementById("TestForm").addEventListener("submit", function (event) {
	var isValid = true;
	var answerLists = document.querySelectorAll('.answer-list');

	answerLists.forEach(function (list) {
		var radios = list.querySelectorAll('input[type="radio"]');
		var isChecked = false;
		radios.forEach(function (radio) {
			if (radio.checked) {
				isChecked = true;
			}
		});
		if (!isChecked) {
			isValid = false;
			list.classList.add("answer-list-error"); // Highlight answer list that is missing selection
		} else {
			list.classList.remove("answer-list-error");
		}
	});

	if (!isValid) {
		event.preventDefault(); // Prevent form submission
		document.getElementById("testform_error-message").classList.remove("d-none"); // Show error message
	}
});

