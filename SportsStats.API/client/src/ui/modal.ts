import { el } from './utils';

export interface ModalHandle {
  root: HTMLElement;
  body: HTMLElement;
  close: () => void;
}

export function openModal(title: string, contentHtml: string, opts?: { wide?: boolean; tall?: boolean }): ModalHandle {
  const dialogCls = ['app-modal__dialog', opts?.wide && 'app-modal__dialog--wide', opts?.tall && 'app-modal__dialog--tall']
    .filter(Boolean)
    .join(' ');
  const root = el(`
    <div class="app-modal app-modal--open">
      <div class="app-modal__backdrop" data-close></div>
      <div class="${dialogCls}">
        <header class="app-modal__head">
          <h3>${title}</h3>
          <button class="icon-btn" type="button" data-close title="Закрыть">×</button>
        </header>
        <div class="app-modal__body">${contentHtml}</div>
      </div>
    </div>
  `);
  const body = root.querySelector<HTMLElement>('.app-modal__body')!;
  const close = () => {
    root.remove();
    document.body.classList.remove('no-scroll');
  };
  root.querySelectorAll<HTMLElement>('[data-close]').forEach((x) => x.addEventListener('click', close));
  document.body.classList.add('no-scroll');
  document.body.appendChild(root);
  return { root, body, close };
}
