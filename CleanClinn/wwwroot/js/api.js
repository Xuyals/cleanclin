// ============================================================
//  api.js — Todas las llamadas al backend de Clean Clinn
//  El compañero frontend solo importa este archivo
// ============================================================

const BASE_URL = "http://localhost:5000/api";

// Guarda el token en localStorage
function getToken() {
    return localStorage.getItem("token");
}

function authHeaders() {
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    };
}

// ── AUTH ──────────────────────────────────────────────────────

export async function login(email, password) {
    const res = await fetch(`${BASE_URL}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje || "Error al iniciar sesión");

    localStorage.setItem("token", data.token);
    localStorage.setItem("rol", data.rol);
    localStorage.setItem("nombre", data.nombre);
    localStorage.setItem("usuarioId", data.usuarioId);

    return data;
}

export async function register(datos) {
    const res = await fetch(`${BASE_URL}/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(datos)
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje || "Error al registrar");
    return data;
}

export function logout() {
    localStorage.clear();
    window.location.href = "/login.html";
}

// ── SOLICITUDES ───────────────────────────────────────────────

export async function getSolicitudes() {
    const res = await fetch(`${BASE_URL}/solicitudes`, {
        headers: authHeaders()
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje);
    return data;
}

export async function crearSolicitud(tipoServicioId, direccion, descripcion) {
    const res = await fetch(`${BASE_URL}/solicitudes`, {
        method: "POST",
        headers: authHeaders(),
        body: JSON.stringify({ tipoServicioId, direccion, descripcion })
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje);
    return data;
}

export async function aceptarSolicitud(id) {
    const res = await fetch(`${BASE_URL}/solicitudes/${id}/aceptar`, {
        method: "PUT",
        headers: authHeaders()
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje);
    return data;
}

export async function completarSolicitud(id) {
    const res = await fetch(`${BASE_URL}/solicitudes/${id}/completar`, {
        method: "PUT",
        headers: authHeaders()
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje);
    return data;
}

export async function cancelarSolicitud(id, motivo) {
    const res = await fetch(`${BASE_URL}/solicitudes/${id}/cancelar`, {
        method: "PUT",
        headers: authHeaders(),
        body: JSON.stringify(motivo)
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje);
    return data;
}

// ── TRABAJADORES ──────────────────────────────────────────────

export async function getTrabajadores(especialidad = "") {
    const url = especialidad
        ? `${BASE_URL}/trabajadores?especialidad=${especialidad}`
        : `${BASE_URL}/trabajadores`;
    const res = await fetch(url);
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje);
    return data;
}

// ── CALIFICACIONES ────────────────────────────────────────────

export async function calificar(solicitudId, puntaje, comentario) {
    const res = await fetch(`${BASE_URL}/calificaciones`, {
        method: "POST",
        headers: authHeaders(),
        body: JSON.stringify({ solicitudId, puntaje, comentario })
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mensaje);
    return data;
}

export async function getCalificacionesTrabajador(trabajadorId) {
    const res = await fetch(`${BASE_URL}/calificaciones/trabajador/${trabajadorId}`);
    return await res.json();
}

// ── HELPERS ───────────────────────────────────────────────────

export function estaLogueado() {
    return !!getToken();
}

export function getRol() {
    return localStorage.getItem("rol");
}

export function getNombre() {
    return localStorage.getItem("nombre");
}