# ConectAR — Gestor de Ventas y Compras

Trabajo de Diploma (T1-23-32) — UAI, Facultad de Tecnología Informática
**Alumno:** Villaverde, Agustín — Legajo B00033339-T1
**Comisión:** 3°BN — Turno noche — Sede Centro — Cursada 2026
**Docentes:** Ing. Pereyra, Jorge Agustín — Ing. Silvestro, Agustín

Sistema de gestión para **ConectAR S.R.L.**, distribuidora de materiales para
cableado estructurado y conectividad. Windows Forms + C# + SQL Server, en capas,
con acceso por ADO.NET y stored procedures.

---

## Estructura

```
Entidad_BE/      entidades
Acceso_DAL/      acceso a datos (ADO.NET + stored procedures)
Negocio_BLL/     lógica de negocio
Servicios/       transversales: encriptación, logger, DV, idioma, sesión
Presentacion/    formularios Windows Forms
db/              scripts de base de datos
docs/            documentación por entrega
```

## Primer inicio de sesión

| | |
|-|-|
| Usuario | `admin` |
| Clave | `Admin1234` |

Si se modifican a mano los datos del usuario administrador, hay que recalcular
los dígitos verificadores desde *Recalcular Dígitos* (CUS-012); de lo contrario
el control de integridad del login va a fallar.

---

## Nomenclatura

Las clases y formularios del **negocio** llevan sufijo `575_AV` (tres últimos
dígitos del DNI + iniciales), según la regla de la cátedra. Los servicios
transversales conservan los nombres del trabajo práctico ya entregado.

## Ramas

`main` y la rama de la entrega en curso. Cada rama `entrega-N` se crea al
comenzar esa entrega, no antes: hoy existen `main` y `entrega-1`.

Se trabaja en la rama de la entrega en curso; al aprobarse se mergea a `main`
y se etiqueta `entrega-N-aprobada`. **Las ramas no se borran**: el docente las
revisa.
