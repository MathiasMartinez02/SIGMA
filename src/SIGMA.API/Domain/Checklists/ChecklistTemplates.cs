using SIGMA.API.Enums;

namespace SIGMA.API.Checklists;

public record ChecklistItemTemplate(string Code, string Description, string? Reference = null);

public record ChecklistSectionTemplate(string Title, IReadOnlyList<ChecklistItemTemplate> Items);

public static class ChecklistTemplates
{
    private const string Raac43 = "RAAC 43 Ap. D";
    private const string Fabricante = "Manual de mantenimiento del fabricante";

    private static readonly ChecklistSectionTemplate Documentacion = new("Documentación", [
        new("DOC-01", "Certificado de aeronavegabilidad y matrícula vigentes", Raac43),
        new("DOC-02", "Directivas de aeronavegabilidad aplicables verificadas", Raac43),
        new("DOC-03", "Historial y libros de la aeronave actualizados", Raac43),
    ]);

    private static readonly ChecklistSectionTemplate Fuselaje = new("Fuselaje y cabina", [
        new("FUS-01", "Revestimiento sin daños, corrosión ni deformaciones", Raac43),
        new("FUS-02", "Puertas, ventanillas y cierres en buen estado", Raac43),
        new("FUS-03", "Asientos, cinturones y arneses en buen estado y asegurados", Raac43),
        new("FUS-04", "Comandos de vuelo con recorrido libre y correcto", Raac43),
        new("FUS-05", "Instrumentos sin daños y correctamente instalados", Raac43),
    ]);

    private static readonly ChecklistSectionTemplate Motor = new("Grupo motor", [
        new("MOT-01", "Cambio de aceite y control del filtro / pantalla", Fabricante),
        new("MOT-02", "Bancada y montantes del motor sin fisuras", Raac43),
        new("MOT-03", "Compresión de cilindros dentro de límites", Raac43),
        new("MOT-04", "Bujías y cables de encendido en buen estado", Raac43),
        new("MOT-05", "Escape sin fisuras ni pérdidas", Raac43),
        new("MOT-06", "Líneas de combustible y aceite sin pérdidas", Raac43),
        new("MOT-07", "Magnetos con puesta a punto correcta", Raac43),
    ]);

    private static readonly ChecklistSectionTemplate Tren = new("Tren de aterrizaje", [
        new("TRE-01", "Neumáticos con desgaste y presión dentro de límites", Raac43),
        new("TRE-02", "Frenos sin pérdidas y con desgaste dentro de límites", Raac43),
        new("TRE-03", "Amortiguadores y estructura del tren en buen estado", Raac43),
    ]);

    private static readonly ChecklistSectionTemplate Alas = new("Alas y empenaje", [
        new("ALA-01", "Revestimiento y estructura de alas sin daños", Raac43),
        new("ALA-02", "Superficies de control y bisagras en buen estado", Raac43),
        new("ALA-03", "Cables, poleas y tensiones de comandos correctos", Raac43),
    ]);

    private static readonly ChecklistSectionTemplate Helice = new("Hélice", [
        new("HEL-01", "Palas sin muescas, fisuras ni corrosión", Raac43),
        new("HEL-02", "Bulones de fijación con torque correcto", Raac43),
    ]);

    private static readonly ChecklistSectionTemplate Electrico = new("Sistemas eléctricos y aviónica", [
        new("ELE-01", "Batería, cableado y protecciones en buen estado", Raac43),
        new("ELE-02", "Equipos de radio y navegación operativos", Raac43),
        new("ELE-03", "Luces de navegación, anticolisión y aterrizaje operativas", Raac43),
    ]);

    private static readonly ChecklistSectionTemplate Cierre = new("Prueba y cierre", [
        new("CIE-01", "Prueba de motor en tierra satisfactoria"),
        new("CIE-02", "Trabajos registrados en el libro de la aeronave"),
    ]);

    private static readonly ChecklistSectionTemplate Conformidad = new("Conformidad de los trabajos", [
        new("CON-01", "Trabajos realizados según datos técnicos aprobados"),
        new("CON-02", "Repuestos y materiales con certificado de conformidad"),
        new("CON-03", "Prueba funcional del sistema intervenido satisfactoria"),
        new("CON-04", "Trabajos registrados en el libro de la aeronave"),
    ]);

    private static readonly ChecklistSectionTemplate DirectivaAd = new("Cumplimiento de directiva", [
        new("AD-01", "Directiva aplicable identificada por número y revisión"),
        new("AD-02", "Método de cumplimiento ejecutado según la directiva"),
        new("AD-03", "Cumplimiento registrado con próxima repetición si corresponde"),
    ]);

    // Plantilla de checklist segun el tipo de OT de la inspeccion
    public static IReadOnlyList<ChecklistSectionTemplate> For(string inspectionType) =>
        Enum.TryParse<WorkOrderType>(inspectionType, out var type) ? For(type) : [Conformidad];

    // Plantilla de checklist segun el tipo de OT
    public static IReadOnlyList<ChecklistSectionTemplate> For(WorkOrderType type) => type switch
    {
        WorkOrderType.Inspeccion50h => [Motor, Tren, Cierre],
        WorkOrderType.Inspeccion100h or WorkOrderType.InspeccionAnual or WorkOrderType.Inspeccion200h =>
            [Documentacion, Fuselaje, Motor, Tren, Alas, Helice, Electrico, Cierre],
        WorkOrderType.AdCumplimiento => [DirectivaAd, Conformidad],
        _ => [Conformidad],
    };
}
