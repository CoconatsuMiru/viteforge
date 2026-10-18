using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services.Integrations;

public class ReactRouterIntegration : IIntegration
{
    private const string MainTsx = """
        import { StrictMode } from 'react'
        import { createRoot } from 'react-dom/client'
        import { BrowserRouter } from 'react-router'
        import './index.css'
        import App from './App'

        createRoot(document.getElementById('root')!).render(
          <StrictMode>
            <BrowserRouter>
              <App />
            </BrowserRouter>
          </StrictMode>,
        )
        """;

    private const string AppTsx = """
        import { Route, Routes } from 'react-router'
        import Layout from './components/Layout'
        import About from './pages/About'
        import Home from './pages/Home'
        import NotFound from './pages/NotFound'

        export default function App() {
          return (
            <Routes>
              <Route element={<Layout />}>
                <Route index element={<Home />} />
                <Route path="about" element={<About />} />
                <Route path="*" element={<NotFound />} />
              </Route>
            </Routes>
          )
        }
        """;

    private const string LayoutTsx = """
        import { Link, Outlet } from 'react-router'

        export default function Layout() {
          return (
            <>
              <nav>
                <Link to="/">Home</Link> | <Link to="/about">About</Link>
              </nav>
              <main>
                <Outlet />
              </main>
            </>
          )
        }
        """;

    private const string HomeTsx = """
        export default function Home() {
          return <h1>Home</h1>
        }
        """;

    private const string AboutTsx = """
        export default function About() {
          return <h1>About</h1>
        }
        """;

    private const string NotFoundTsx = """
        import { Link } from 'react-router'

        export default function NotFound() {
          return (
            <>
              <h1>Page not found</h1>
              <Link to="/">Back to home</Link>
            </>
          )
        }
        """;

    public string Name => "React Router";

    public bool IsSelected(ProjectOptions options) => options.UseReactRouter;

    public bool Apply(ProjectOptions options)
    {
        if (!PackageInstaller.Add(options, "react-router"))
        {
            ConsoleUI.Error("Could not install React Router.");
            return false;
        }

        var root = options.ProjectPath;

        WriteFile(root, MainTsx, "src", "main.tsx");
        WriteFile(root, AppTsx, "src", "App.tsx");
        WriteFile(root, LayoutTsx, "src", "components", "Layout.tsx");
        WriteFile(root, HomeTsx, "src", "pages", "Home.tsx");
        WriteFile(root, AboutTsx, "src", "pages", "About.tsx");
        WriteFile(root, NotFoundTsx, "src", "pages", "NotFound.tsx");

        return true;
    }

    private static void WriteFile(string projectPath, string content, params string[] parts)
    {
        var path = Path.Combine(new[] { projectPath }.Concat(parts).ToArray());

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content + "\n");
    }
}