"""
Script en Python para extraer los puntos geométricos (polígono) y la matriz de colores
a partir de la imagen 'avion.png' para el juego en Windows Forms (Form1.cs).
"""

from PIL import Image
import numpy as np
import os

def rdp(points, epsilon=1.2):
    """Algoritmo Ramer-Douglas-Peucker para simplificar polígonos."""
    if len(points) < 3:
        return points
    start = np.array(points[0])
    end = np.array(points[-1])
    line_vec = end - start
    line_len = np.linalg.norm(line_vec)
    
    if line_len == 0:
        dists = np.linalg.norm(points - start, axis=1)
    else:
        line_unit = line_vec / line_len
        vecs = points - start
        proj = np.dot(vecs, line_unit)
        proj = np.clip(proj, 0, line_len)
        closest_pts = start + np.outer(proj, line_unit)
        dists = np.linalg.norm(points - closest_pts, axis=1)
        
    dmax = np.max(dists)
    index = np.argmax(dists)
    
    if dmax > epsilon:
        rec1 = rdp(points[:index+1], epsilon)
        rec2 = rdp(points[index:], epsilon)
        return np.vstack((rec1[:-1], rec2))
    else:
        return np.array([points[0], points[-1]])

def get_contour(binary_mask):
    """Extrae el contorno exterior usando Moore-Neighbor tracing."""
    pad = np.pad(binary_mask, 1, mode='constant', constant_values=0)
    start_pts = np.argwhere(pad)
    if len(start_pts) == 0:
        return []
    start_y, start_x = start_pts[0]
    
    directions = [(-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1)]
    contour = []
    
    curr = (start_y, start_x)
    backtrack = 0
    contour.append((curr[1]-1, curr[0]-1))
    
    for _ in range(5000):
        found = False
        for i in range(8):
            idx = (backtrack + i) % 8
            dy, dx = directions[idx]
            ny, nx = curr[0] + dy, curr[1] + dx
            if 0 <= ny < pad.shape[0] and 0 <= nx < pad.shape[1] and pad[ny, nx]:
                curr = (ny, nx)
                contour.append((curr[1]-1, curr[0]-1))
                backtrack = (idx + 5) % 8
                found = True
                break
        if not found or (curr[0] == start_y and curr[1] == start_x and len(contour) > 2):
            break
            
    if len(contour) > 0 and (contour[0] != contour[-1]):
        contour.append(contour[0])
        
    return np.array(contour)

def procesar_avion(image_path="avion.png", target_w=58, target_h=77, epsilon=1.2):
    if not os.path.exists(image_path):
        print(f"Error: No se encontró el archivo '{image_path}'.")
        return

    img = Image.open(image_path).convert("RGBA")
    arr = np.array(img)
    r, g, b, a = arr[:,:,0], arr[:,:,1], arr[:,:,2], arr[:,:,3]

    # Filtrar fondo blanco / transparente / gris claro
    is_fg = (a > 50) & ~((r > 210) & (g > 210) & (b > 210))
    coords = np.argwhere(is_fg)

    if len(coords) == 0:
        print("No se encontraron píxeles de la nave.")
        return

    min_y, min_x = coords.min(axis=0)
    max_y, max_x = coords.max(axis=0)
    cropped = img.crop((min_x, min_y, max_x+1, max_y+1))

    # Redimensionar
    resized = cropped.resize((target_w, target_h), Image.Resampling.LANCZOS)
    arr_res = np.array(resized)
    r_res, g_res, b_res, a_res = arr_res[:,:,0], arr_res[:,:,1], arr_res[:,:,2], arr_res[:,:,3]
    mask_res = (a_res > 50) & ~((r_res > 210) & (g_res > 210) & (b_res > 210))

    # 1. Puntos del contorno (polígono)
    raw_contour = get_contour(mask_res)
    simplified_contour = rdp(raw_contour, epsilon=epsilon)
    cs_points = ", ".join([f"new Point({p[0]}, {p[1]})" for p in simplified_contour])

    # 2. Puntos con colores para dibujo
    colores_dict = {}
    for y in range(target_h):
        for x in range(target_w):
            if mask_res[y, x]:
                # Cuantización de color para optimizar rendimiento
                cr = int(round(r_res[y, x] / 32) * 32)
                cg = int(round(g_res[y, x] / 32) * 32)
                cb = int(round(b_res[y, x] / 32) * 32)
                col_key = (min(255, cr), min(255, cg), min(255, cb))
                if col_key not in colores_dict:
                    colores_dict[col_key] = []
                colores_dict[col_key].append((x, y))

    # Generación de código para Form1.cs
    output_code = f"""// ==================================================================================
// CÓDIGO C# LISTO PARA Form1.cs
// Extraído de: {image_path} | Dimensiones: anchoN = {target_w}, largoN = {target_h}
// ==================================================================================

// 1. Array de puntos para el contorno de la nave (Polygon / Region):
Point[] myNaveCustom = {{ {cs_points} }};

// 2. Arrays de puntos y colores para pintar la textura del avión:
(Color color, Point[] puntos)[] capasColores = new (Color, Point[])[]
{{
"""
    for (cr, cg, cb), pts in colores_dict.items():
        pts_str = ", ".join([f"new Point({px}, {py})" for px, py in pts])
        output_code += f"    (Color.FromArgb({cr}, {cg}, {cb}), new Point[] {{ {pts_str} }}),\n"

    output_code += f"""}};

// 3. Método para dibujar los colores en la imagen de la nave:
public void PintarTexturaAvion(Graphics g)
{{
    foreach (var capa in capasColores)
    {{
        using (SolidBrush brush = new SolidBrush(capa.color))
        {{
            foreach (var p in capa.puntos)
            {{
                g.FillRectangle(brush, p.X, p.Y, 1, 1);
            }}
        }}
    }}
}}
"""

    with open("avion_codigo_cs.txt", "w", encoding="utf-8") as f:
        f.write(output_code)

    print("=========================================================")
    print("¡EXTRACCIÓN EXITOSA!")
    print(f"- Puntos del contorno calculados: {len(simplified_contour)}")
    print(f"- Capas de color agrupadas: {len(colores_dict)}")
    print(f"- Archivo con el código C# generado: 'avion_codigo_cs.txt'")
    print("=========================================================")

if __name__ == "__main__":
    procesar_avion("avion.png", target_w=58, target_h=77, epsilon=1.2)
