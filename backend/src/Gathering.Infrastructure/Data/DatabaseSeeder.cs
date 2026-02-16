using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Gathering.Infrastructure.Data;

public class DatabaseSeeder
{
  private readonly ApplicationDbContext _context;

  public DatabaseSeeder(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task SeedAsync()
  {
    // Check if data already exists
    if (await _context.Communities.AnyAsync())
    {
      return; // Database already seeded
    }

    await SeedCommunitiesAsync();
    await SeedSessionsAsync();

    await _context.SaveChangesAsync();
  }

  private async Task SeedCommunitiesAsync()
  {
    var frontendCommunity = Community.Create(
        "Frontend Development",
        "Comunidad enfocada en desarrollo frontend, frameworks modernos, UI/UX y mejores prácticas de desarrollo web",
        "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=800"
    );

    var pythonCommunity = Community.Create(
        "Python & Data Science",
        "Comunidad dedicada a Python, análisis de datos, machine learning y desarrollo backend",
        "https://images.unsplash.com/photo-1526379095098-d400fd0bf935?w=800"
    );

    var cloudCommunity = Community.Create(
        "Cloud & DevOps",
        "Comunidad especializada en cloud computing, arquitecturas escalables, CI/CD y prácticas DevOps",
        "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=800"
    );

    if (frontendCommunity.IsSuccess)
    {
      await _context.Communities.AddAsync(frontendCommunity.Value);
    }

    if (pythonCommunity.IsSuccess)
    {
      await _context.Communities.AddAsync(pythonCommunity.Value);
    }

    if (cloudCommunity.IsSuccess)
    {
      await _context.Communities.AddAsync(cloudCommunity.Value);
    }

    await _context.SaveChangesAsync();
  }

  private async Task SeedSessionsAsync()
  {
    var communities = await _context.Communities.ToListAsync();
    var frontendCommunity = communities.First(c => c.Name == "Frontend Development");
    var pythonCommunity = communities.First(c => c.Name == "Python & Data Science");
    var cloudCommunity = communities.First(c => c.Name == "Cloud & DevOps");

    // Frontend Sessions
    await SeedFrontendSessionsAsync(frontendCommunity.Id);

    // Python Sessions
    await SeedPythonSessionsAsync(pythonCommunity.Id);

    // Cloud Sessions
    await SeedCloudSessionsAsync(cloudCommunity.Id);
  }

  private async Task SeedFrontendSessionsAsync(Guid communityId)
  {
    var now = DateTime.UtcNow;

    // Session 1: React Hooks Deep Dive (Completed) - 30 days ago
    var session1 = Session.Create(
        communityId,
        "React Hooks Deep Dive",
        "Exploramos en profundidad los hooks de React, patrones avanzados y custom hooks para crear aplicaciones más eficientes y mantenibles.",
        "María González",
        now.AddDays(30),
        "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=800"
    );

    if (session1.IsSuccess)
    {
      session1.Value.UpdateState(SessionState.Completed);

      // Add resources
      session1.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=O6P86uwfdR0",
          null,
          "React Hooks Tutorial - Video Recording"
      );

      session1.Value.AddResource(
          SessionResourceType.Notes,
          null,
          "# React Hooks Deep Dive\n\n## Hooks Principales\n\n### useState\n- Maneja el estado local del componente\n- Ejemplo: const [count, setCount] = useState(0)\n\n### useEffect\n- Maneja efectos secundarios\n- Se ejecuta después del render\n- Puede tener dependencias\n\n### useContext\n- Accede al contexto sin prop drilling\n- Útil para temas, autenticación, etc.\n\n## Custom Hooks\n- Reutiliza lógica entre componentes\n- Siempre empieza con 'use'\n- Ejemplo: useLocalStorage, useFetch\n\n## Mejores Prácticas\n1. No llamar hooks dentro de loops o condiciones\n2. Solo llamar hooks en componentes de React\n3. Usar ESLint plugin para hooks\n4. Mantener hooks simples y enfocados",
          "Notas de la Sesión"
      );

      session1.Value.AddResource(
          SessionResourceType.ExternalLink,
          "https://github.com/facebook/react",
          null,
          "React Official Repository"
      );

      await _context.Sessions.AddAsync(session1.Value);
    }

    // Session 2: TypeScript Best Practices (Scheduled) - 35 days from now
    var session2 = Session.Create(
        communityId,
        "TypeScript Best Practices",
        "Aprende las mejores prácticas de TypeScript, tipos avanzados, generics y cómo aprovechar al máximo el sistema de tipos.",
        "Carlos Ruiz",
        now.AddDays(35),
        "https://images.unsplash.com/photo-1516116216624-53e697fedbea?w=800"
    );

    if (session2.IsSuccess)
    {
      await _context.Sessions.AddAsync(session2.Value);
    }

    // Session 3: Next.js 15 Features (Scheduled) - 55 days from now
    var session3 = Session.Create(
        communityId,
        "Next.js 15: Nuevas Características",
        "Descubre las últimas características de Next.js 15, Server Components, App Router y optimizaciones de rendimiento.",
        "Ana Martínez",
        now.AddDays(55),
        "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800"
    );

    if (session3.IsSuccess)
    {
      await _context.Sessions.AddAsync(session3.Value);
    }

    // Session 4: CSS Grid & Flexbox (Canceled) - 1 day from now (will be canceled)
    var session4 = Session.Create(
        communityId,
        "CSS Grid & Flexbox Masterclass",
        "Domina CSS Grid y Flexbox para crear layouts modernos y responsivos sin frameworks.",
        "Roberto Silva",
        now.AddDays(1),
        "https://images.unsplash.com/photo-1507721999472-8ed4421c4af2?w=800"
    );

    if (session4.IsSuccess)
    {
      session4.Value.UpdateState(SessionState.Canceled);
      await _context.Sessions.AddAsync(session4.Value);
    }
  }

  private async Task SeedPythonSessionsAsync(Guid communityId)
  {
    var now = DateTime.UtcNow;

    // Session 1: Machine Learning Basics (Completed) - 25 days from now
    var session1 = Session.Create(
        communityId,
        "Introducción a Machine Learning con Scikit-learn",
        "Fundamentos de machine learning, algoritmos supervisados y no supervisados con ejemplos prácticos usando Scikit-learn.",
        "Dr. Luis Fernández",
        now.AddDays(25),
        "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=800"
    );

    if (session1.IsSuccess)
    {
      session1.Value.UpdateState(SessionState.Completed);

      // Add resources
      session1.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=7eh4d6sabA0",
          null,
          "Machine Learning con Scikit-learn - Grabación"
      );

      session1.Value.AddResource(
          SessionResourceType.Notes,
          null,
          "# Machine Learning con Scikit-learn\n\n## Tipos de Aprendizaje\n\n### Supervisado\n- Clasificación (SVM, Random Forest, Logistic Regression)\n- Regresión (Linear Regression, Ridge, Lasso)\n\n### No Supervisado\n- Clustering (K-Means, DBSCAN)\n- Reducción de dimensionalidad (PCA, t-SNE)\n\n## Flujo de Trabajo\n1. Cargar datos\n2. Exploración y limpieza\n3. Feature engineering\n4. Split train/test\n5. Entrenamiento\n6. Evaluación\n7. Optimización de hiperparámetros\n\n## Métricas de Evaluación\n- Accuracy\n- Precision, Recall, F1-Score\n- ROC-AUC\n- Confusion Matrix\n\n## Recursos Adicionales\n- Scikit-learn.org\n- Kaggle datasets\n- Google Colab para práctica",
          "Conceptos Clave de ML"
      );

      await _context.Sessions.AddAsync(session1.Value);
    }

    // Session 2: FastAPI Development (Scheduled) - 40 days from now
    var session2 = Session.Create(
        communityId,
        "Desarrollo de APIs con FastAPI",
        "Construye APIs REST modernas y eficientes con FastAPI, incluyendo validación, documentación automática y async/await.",
        "Patricia Torres",
        now.AddDays(40),
        "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800"
    );

    if (session2.IsSuccess)
    {
      await _context.Sessions.AddAsync(session2.Value);
    }

    // Session 3: Data Analysis with Pandas (Completed) - 4 days from now
    var session3 = Session.Create(
        communityId,
        "Análisis de Datos con Pandas",
        "Técnicas avanzadas de análisis y manipulación de datos con Pandas, visualización y limpieza de datasets.",
        "Jorge Mendoza",
        now.AddDays(4),
        "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800"
    );

    if (session3.IsSuccess)
    {
      session3.Value.UpdateState(SessionState.Completed);

      // Add resources
      session3.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=vmEHCJofslg",
          null,
          "Análisis de Datos con Pandas - Sesión Completa"
      );

      session3.Value.AddResource(
          SessionResourceType.ExternalLink,
          "https://colab.research.google.com/",
          null,
          "Google Colab - Notebook Interactivo"
      );

      await _context.Sessions.AddAsync(session3.Value);
    }

    // Session 4: Django for Beginners (Scheduled) - 60 days from now
    var session4 = Session.Create(
        communityId,
        "Django para Principiantes",
        "Primeros pasos con Django, creación de modelos, vistas, templates y administración de usuarios.",
        "Sandra López",
        now.AddDays(60),
        "https://images.unsplash.com/photo-1580927752452-89d86da3fa0a?w=800"
    );

    if (session4.IsSuccess)
    {
      await _context.Sessions.AddAsync(session4.Value);
    }
  }

  private async Task SeedCloudSessionsAsync(Guid communityId)
  {
    var now = DateTime.UtcNow;

    // Session 1: Kubernetes Fundamentals (Completed) - 20 days from now
    var session1 = Session.Create(
        communityId,
        "Fundamentos de Kubernetes",
        "Introducción a Kubernetes, pods, deployments, services y arquitectura de clusters para orquestar contenedores.",
        "Miguel Ángel Rojas",
        now.AddDays(20),
        "https://images.unsplash.com/photo-1667372393119-3d4c48d07fc9?w=800"
    );

    if (session1.IsSuccess)
    {
      session1.Value.UpdateState(SessionState.Completed);

      // Add resources
      session1.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=X48VuDVv0do",
          null,
          "Kubernetes Fundamentals - Recording"
      );

      session1.Value.AddResource(
          SessionResourceType.Notes,
          null,
          "# Kubernetes Fundamentals\n\n## Arquitectura\n\n### Control Plane\n- API Server\n- etcd (almacenamiento)\n- Scheduler\n- Controller Manager\n\n### Worker Nodes\n- kubelet\n- kube-proxy\n- Container runtime (Docker, containerd)\n\n## Objetos Principales\n\n### Pod\n- Unidad básica de deployment\n- Puede contener uno o más contenedores\n- Comparten red y storage\n\n### Deployment\n- Gestiona ReplicaSets\n- Permite actualizaciones rolling\n- Rollback automático\n\n### Service\n- ClusterIP (interno)\n- NodePort (expone puerto)\n- LoadBalancer (cloud)\n\n### ConfigMap & Secret\n- Configuración externalizada\n- Secrets para datos sensibles\n\n## Comandos Básicos\n```bash\nkubectl get pods\nkubectl apply -f deployment.yaml\nkubectl logs <pod-name>\nkubectl exec -it <pod-name> -- /bin/bash\nkubectl scale deployment/app --replicas=3\n```",
          "Guía de Kubernetes"
      );

      session1.Value.AddResource(
          SessionResourceType.ExternalLink,
          "https://kubernetes.io/docs/home/",
          null,
          "Documentación Oficial de Kubernetes"
      );

      await _context.Sessions.AddAsync(session1.Value);
    }

    // Session 2: AWS Serverless Architecture (Scheduled) - 45 days from now
    var session2 = Session.Create(
        communityId,
        "Arquitecturas Serverless en AWS",
        "Diseña y despliega aplicaciones serverless usando AWS Lambda, API Gateway, DynamoDB y otros servicios de AWS.",
        "Laura Jiménez",
        now.AddDays(45),
        "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=800"
    );

    if (session2.IsSuccess)
    {
      await _context.Sessions.AddAsync(session2.Value);
    }

    // Session 3: Docker Deep Dive (Completed) - 1 day from now
    var session3 = Session.Create(
        communityId,
        "Docker: De Básico a Avanzado",
        "Domina Docker: imágenes, contenedores, networking, volumes, Docker Compose y optimización de builds.",
        "Fernando Castro",
        now.AddDays(1),
        "https://images.unsplash.com/photo-1605745341075-0f92a4f2c01f?w=800"
    );

    if (session3.IsSuccess)
    {
      session3.Value.UpdateState(SessionState.Completed);

      // Add resources
      session3.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=fqMOX6JJhGo",
          null,
          "Docker Deep Dive - Full Session"
      );

      session3.Value.AddResource(
          SessionResourceType.ExternalLink,
          "https://docs.docker.com/get-started/",
          null,
          "Docker Get Started Guide"
      );

      await _context.Sessions.AddAsync(session3.Value);
    }

    // Session 4: Terraform Infrastructure as Code (Scheduled) - 65 days from now
    var session4 = Session.Create(
        communityId,
        "Infrastructure as Code con Terraform",
        "Aprende a gestionar infraestructura como código usando Terraform, módulos, state management y mejores prácticas.",
        "Elena Vargas",
        now.AddDays(65),
        "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800"
    );

    if (session4.IsSuccess)
    {
      await _context.Sessions.AddAsync(session4.Value);
    }

    // Session 5: CI/CD with GitHub Actions (Canceled) - 14 days from now (will be canceled)
    var session5 = Session.Create(
        communityId,
        "CI/CD con GitHub Actions",
        "Automatiza tus flujos de trabajo con GitHub Actions, workflows, secrets y deployment strategies.",
        "Ricardo Peña",
        now.AddDays(14),
        "https://images.unsplash.com/photo-1618401471353-b98afee0b2eb?w=800"
    );

    if (session5.IsSuccess)
    {
      session5.Value.UpdateState(SessionState.Canceled);
      await _context.Sessions.AddAsync(session5.Value);
    }
  }
}
