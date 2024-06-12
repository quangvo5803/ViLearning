// Show options if question type is multiple choice
document.getElementById('questionType').addEventListener('change', function () {
    var multipleChoiceOptions = document.getElementById('multipleChoiceOptions');
    if (this.value === 'MultipleChoice') {
        multipleChoiceOptions.classList.remove('d-none');
    } else {
        multipleChoiceOptions.classList.add('d-none');
    }
});


// Show options if question type is multiple choice for update modal
document.addEventListener("DOMContentLoaded", function () {
    var questionTypeSelects = document.querySelectorAll(".questionType_update");
    var multipleChoiceOptionsSections = document.querySelectorAll(".multipleChoiceOptions_update");

    function toggleOptions(selectElement, optionsSection) {
        if (selectElement.value === "MultipleChoice") {
            optionsSection.classList.remove("d-none");
        } else {
            optionsSection.classList.add("d-none");
        }
    }

    questionTypeSelects.forEach(function (selectElement, index) {
        var correspondingOptionsSection = multipleChoiceOptionsSections[index];
        // Initial check
        toggleOptions(selectElement, correspondingOptionsSection);

        // Add event listener
        selectElement.addEventListener("change", function () {
            toggleOptions(selectElement, correspondingOptionsSection);
        });
    });
});

// Make sure client choose one of option in update modal
var updateForms = document.querySelectorAll(".updateQuestionForm");
updateForms.forEach(function (form) {
    form.addEventListener("submit", function (event) {
        var questionType = form.querySelector(".questionType_update").value;
        var isChecked = false;

        if (questionType === "MultipleChoice") {
            var options = form.querySelectorAll(".right-answer");
            for (var i = 0; i < options.length; i++) {
                if (options[i].checked) {
                    isChecked = true;
                    break;
                }
            }
        } else {
            isChecked = true; // For non-MultipleChoice questions, no need to check further
        }

        var errorMessage = form.querySelector(".error-message");
        if (!isChecked) {
            errorMessage.classList.remove("d-none"); // Hiển thị thông báo lỗi
            event.preventDefault(); // Ngăn chặn gửi form
        } else {
            errorMessage.classList.add("d-none"); // Ẩn thông báo lỗi nếu đã chọn
        }
    });
});

// Make sure client choose one of option in add modal
document.getElementById("addQuestionForm").addEventListener("submit", function (event) {
    var questionType = document.getElementById("questionType").value;
    var isChecked = false;

    if (questionType === "MultipleChoice") {
        var options = document.getElementsByName("Question.RightAnswer");
        for (var i = 0; i < options.length; i++) {
            if (options[i].checked) {
                isChecked = true;
                break;
            }
        }
    } else {
        isChecked = true; // For non-MultipleChoice questions, no need to check further
    }
    var addModal_errorMessage = document.getElementById('addModal_error-message');
    if (!isChecked) {
        addModal_errorMessage.classList.remove("d-none");
        event.preventDefault(); // Prevent form submission
    } else {
        addModal_errorMessage.classList.add("d-none"); // Ẩn thông báo lỗi nếu đã chọn
    }
});

// Change Difficulty col color
const difficultyCells = document.querySelectorAll('.list_difficulty');
difficultyCells.forEach(function (cell) {
    // Get the text content of the cell
    const difficulty = cell.textContent.trim();

    // Add a class based on the difficulty level
    if (difficulty === 'Easy') {
        cell.classList.add('text-success');
    } else if (difficulty === 'Medium') {
        cell.classList.add('text-warning');
    } else if (difficulty === 'Hard') {
        cell.classList.add('text-danger');
    }
});
