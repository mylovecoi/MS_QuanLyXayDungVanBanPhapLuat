import React from 'react';
import { createRoot } from 'react-dom/client';
import './styles.css';

const modules = [
  {
    title: 'Quan tri he thong',
    description: 'Nguoi dung, nhom quyen, vai tro, phan quyen va cau hinh he thong.',
    path: '/quan-tri-he-thong'
  },
  {
    title: 'Danh muc',
    description: 'Don vi, loai van ban, quy trinh soan thao, buoc va chuyen buoc.',
    path: '/danh-muc'
  }
];

function App() {
  return (
    <main className="shell">
      <aside className="sidebar">
        <div className="brand">MS</div>
        <nav>
          {modules.map((module) => (
            <a key={module.path} href={module.path}>{module.title}</a>
          ))}
        </nav>
      </aside>
      <section className="content">
        <header>
          <p className="eyebrow">MS_QuanLyXayDungVanBanPhapLuat</p>
          <h1>Khoi dong lai he thong</h1>
          <p>Frontend ReactJS moi cho cac service Quan tri he thong va Danh muc.</p>
        </header>
        <div className="module-grid">
          {modules.map((module) => (
            <article className="module-card" key={module.path}>
              <h2>{module.title}</h2>
              <p>{module.description}</p>
              <a href={module.path}>Mo module</a>
            </article>
          ))}
        </div>
      </section>
    </main>
  );
}

createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
