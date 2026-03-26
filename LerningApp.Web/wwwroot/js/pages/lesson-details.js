document.addEventListener('DOMContentLoaded', () => {
    initMultipleChoiceCheck();
    initMultipleChoiceDelete();
    initListeningCheck();
    initTranslationLanguageToggle();
    initTranslationCheck();
    initTranslationDelete();
});

function initMultipleChoiceCheck() {
    document.querySelectorAll('.exercise-box-multiple-choice').forEach(box => {
        const checkBtn = box.querySelector('.check-btn');
        if (!checkBtn) return;

        checkBtn.addEventListener('click', async () => {
            box.querySelectorAll('.answer-row').forEach(r => r.classList.remove('correct', 'wrong'));

            const selected = box.querySelector('input[type="radio"]:checked');
            if (!selected) return;

            const exerciseId = box.querySelector('input[name="exerciseId"]').value;
            const lessonId = box.querySelector('input[name="lessonId"]').value;
            const token = box.querySelector('input[name="__RequestVerificationToken"]').value;

            const payload = {
                exerciseId: exerciseId,
                selectedAnswer: selected.value,
                lessonId: lessonId
            };

            const res = await fetch('https://localhost:7092/api/multiple-choice-exercise/check-answer', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                credentials: 'include',
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const err = await res.json().catch(() => ({}));
                console.log(err.message || 'Invalid operation.');
                toastr.error(err.message || 'Invalid operation.');
                return;
            }

            const data = await res.json();
            const selectedRow = selected.closest('.answer-row');

            if (data.isCorrect) {
                selectedRow.classList.add('correct');
            } else {
                selectedRow.classList.add('wrong');
                box.querySelectorAll('input[type="radio"]').forEach(r => {
                    if (r.value === data.correctAnswer) {
                        r.closest('.answer-row').classList.add('correct');
                    }
                });
            }
        });
    });
}

function initMultipleChoiceDelete() {
    document.querySelectorAll('.multiple-exercise-admin').forEach(adm => {
        const delBtn = adm.querySelector('.btn-sm');
        if (!delBtn) return;

        delBtn.addEventListener('click', async () => {
            const token = adm.querySelector('input[name="__RequestVerificationToken"]').value;
            const exerciseId = adm.querySelector('input[name="id"]').value;

            const payload = {
                exerciseId: exerciseId
            };

            if (!confirm('Сигурни ли сте, че искате да изтриете това упражнение?')) {
                return;
            }

            const res = await fetch('https://localhost:7092/api/multiple-choice-exercise/soft-delete', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                credentials: 'include',
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const text = await res.text();
                console.log(text);
                toastr.error(text || 'Invalid operation.');
                return;
            }

            const text = await res.text();
            toastr.success(text);
            adm.closest('.exercise-shell')?.remove();
        });
    });
}

function initListeningCheck() {
    document.querySelectorAll('.exercise-box-listening').forEach(box => {
        const checkBtn = box.querySelector('.check-listening-btn');
        if (!checkBtn) return;

        checkBtn.addEventListener('click', async () => {
            const exerciseWrap = checkBtn.closest('.listening-exercise-box');
            if (!exerciseWrap) return;

            exerciseWrap.querySelectorAll('.answer-row').forEach(r => {
                r.classList.remove('correct', 'wrong');
            });

            const lessonId = box.querySelector('input[name="lessonId"]').value;
            const exerciseId = box.querySelector('input[name="exerciseId"]').value;
            const token = box.querySelector('input[name="__RequestVerificationToken"]').value;

            const answers = [];
            exerciseWrap.querySelectorAll('.listening-question').forEach(q => {
                const questionId = q.querySelector('input[name="questionId"]').value;
                const selected = q.querySelector('input[type="radio"]:checked');
                if (selected) {
                    answers.push({ questionId, selectedAnswer: selected.value });
                }
            });

            const payload = {
                exerciseId: exerciseId,
                lessonId: lessonId,
                answers: answers
            };

            const res = await fetch('https://localhost:7092/api/listening-exercise/check-answer', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                credentials: 'include',
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const err = await res.json().catch(() => ({}));
                console.log(err.message || 'Invalid operation.');
                toastr.error(err.message || 'Invalid operation.');
                return;
            }

            const data = await res.json();

            exerciseWrap.querySelectorAll('.listening-question').forEach(q => {
                const questionId = q.querySelector('input[name="questionId"]').value;
                const selected = q.querySelector('input[type="radio"]:checked');
                if (!selected) return;

                const row = selected.closest('.answer-row');
                row.classList.remove('correct', 'wrong');

                const resultItem = data.find(x =>
                    x.questionId?.toLowerCase() === questionId.toLowerCase()
                );

                if (resultItem?.isCorrect === true) {
                    row.classList.add('correct');
                } else {
                    row.classList.add('wrong');
                }
            });
        });
    });
}

function initTranslationLanguageToggle() {
    document.querySelectorAll('.exercise-box-translation').forEach(box => {
        const question = box.querySelector('.question');
        const radios = box.querySelectorAll('input[name="selectedLanguage"]');
        if (!question || radios.length === 0) return;

        radios.forEach(r => {
            r.addEventListener('change', () => {
                question.textContent = r.value === 'Bg'
                    ? question.dataset.bg
                    : question.dataset.en;
            });
        });
    });
}
function initTranslationDelete() {
    document.querySelectorAll('.translation-exercise-admin').forEach(adm => {
        const delBtn = adm.querySelector('.btn-sm');
        if (!delBtn) return;

        delBtn.addEventListener('click', async () => {
            const token = adm.querySelector('input[name="__RequestVerificationToken"]').value;
            const exerciseId = adm.querySelector('input[name="id"]').value;

            const payload = {
                exerciseId: exerciseId
            };

            if (!confirm('Сигурни ли сте, че искате да изтриете това упражнение?')) {
                return;
            }

            const res = await fetch('https://localhost:7092/api/translation-exercise/soft-delete', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                credentials: 'include',
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const text = await res.text();
                console.log(text);
                toastr.error(text || 'Invalid operation.');
                return;
            }

            const text = await res.text();
            toastr.success(text);
            adm.closest('.exercise-shell')?.remove();
        });
    });
}
function initTranslationCheck() {
    document.querySelectorAll('.exercise-box-translation').forEach(box => {
        const btn = box.querySelector('.check-translation-btn');
        if (!btn) return;

        btn.addEventListener('click', async () => {
            const exerciseId = box.querySelector('input[name="exerciseId"]').value;
            const token = box.querySelector('input[name="__RequestVerificationToken"]').value;
            const userAnswer = box.querySelector('input[name="userAnswer"]').value;
            const lessonId = box.querySelector('input[name="lessonId"]').value;

            const payload = {
                exerciseId: exerciseId,
                userTranslation: userAnswer,
                lessonId: lessonId
            };

            const res = await fetch('https://localhost:7092/api/translation-exercise/check-answer', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                credentials: 'include',
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const err = await res.json().catch(() => ({}));
                console.log(err.message || 'Invalid operation.');
                toastr.error(err.message || 'Invalid operation.');
                return;
            }

            const data = await res.json();
            const input = box.querySelector('input[name="userAnswer"]');
            input.classList.remove('correct', 'wrong');

            if (data.isCorrect) {
                input.classList.add('correct');
            } else {
                input.classList.add('wrong');
            }
        });
    });
}
