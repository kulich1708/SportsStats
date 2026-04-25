const STACK_ID = 'app-error-toast-stack';
const INVALID_CLASS = 'search-input--invalid';

function ensureStack(): HTMLElement {
	let stack = document.getElementById(STACK_ID) as HTMLElement | null;
	if (!stack) {
		stack = document.createElement('div');
		stack.id = STACK_ID;
		stack.className = 'app-error-toast-stack';
		stack.setAttribute('aria-live', 'polite');
		document.body.appendChild(stack);
	}
	return stack;
}

const DEFAULT_TOAST_MS = 5000;

function defaultDurationMs(): number {
	return DEFAULT_TOAST_MS;
}

function pushToast(className: string, message: string, durationMs: number): void {
	const stack = ensureStack();
	const toast = document.createElement('div');
	toast.className = className;
	toast.textContent = message;
	stack.appendChild(toast);

	window.setTimeout(() => {
		toast.remove();
		if (!stack.childElementCount) {
			stack.remove();
		}
	}, durationMs);
}

/** Снять подсветку полей внутри узла (например перед новой попыткой). */
export function clearInvalidHighlightsIn(root: ParentNode): void {
	root.querySelectorAll<HTMLElement>(`.${INVALID_CLASS}`).forEach((el) => {
		el.classList.remove(INVALID_CLASS);
	});
}

export interface ShowValidationToastOptions {
	message: string;
	/** Default 5 seconds. */
	durationMs?: number;
	/** Optional fields to mark invalid. */
	highlightInputs?: readonly (HTMLElement | null | undefined)[];
}


/**
 * Клиентская валидация: сообщение об ошибке ввода + подсветка полей.
 */
export function showValidationToast(options: ShowValidationToastOptions): void {
	const { message, highlightInputs = [] } = options;
	const toHighlight = highlightInputs.filter((el): el is HTMLElement => Boolean(el));
	for (const el of toHighlight) {
		el.classList.add(INVALID_CLASS);
	}

	const durationMs = options.durationMs ?? defaultDurationMs();
	pushToast('app-error-toast', message, durationMs);
}

export interface ShowSuccessToastOptions {
	message: string;
	durationMs?: number;
}

/** Успешное сохранение или создание сущности (зелёный фон). */
export function showSuccessToast(options: ShowSuccessToastOptions): void {
	const durationMs = options.durationMs ?? defaultDurationMs();
	pushToast('app-success-toast', options.message, durationMs);
}

export interface ShowErrorToastOptions {
	message: string;
	durationMs?: number;
}

/** Общее сообщение об ошибке (тот же вид, что и при валидации, без подсветки полей). */
export function showErrorToast(options: ShowErrorToastOptions): void {
	const durationMs = options.durationMs ?? defaultDurationMs();
	pushToast('app-error-toast', options.message, durationMs);
}

export interface ShowDomainErrorToastOptions {
	message: string;
	durationMs?: number;
}

/** @deprecated Используйте showErrorToast */
export function showDomainErrorToast(options: ShowDomainErrorToastOptions): void {
	showErrorToast(options);
}
