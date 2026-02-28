using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Gathering.Infrastructure.Data;

public sealed class DatabaseSeeder
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
        "Community focused on frontend development, modern frameworks, UI/UX and web development best practices",
        "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=800"
    );

    var pythonCommunity = Community.Create(
        "Python & Data Science",
        "Community dedicated to Python, data analysis, machine learning and backend development",
        "https://images.unsplash.com/photo-1526379095098-d400fd0bf935?w=800"
    );

    var cloudCommunity = Community.Create(
        "Cloud & DevOps",
        "Community specialized in cloud computing, scalable architectures, CI/CD and DevOps practices",
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
    var now = DateTimeOffset.UtcNow;

    // Session 1: React Hooks Deep Dive (Completed) - 30 days ago
    var session1 = Session.CreateCompleted(
        communityId,
        "React Hooks Deep Dive",
        "Maria Gonzalez",
        now.AddDays(-30),
        "An in-depth exploration of React hooks, advanced patterns and custom hooks for building more efficient and maintainable applications.",
        "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=800"
    );

    if (session1.IsSuccess)
    {
      session1.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=O6P86uwfdR0",
          null,
          "React Hooks Tutorial - Video Recording"
      );

      session1.Value.AddResource(
          SessionResourceType.Notes,
          null,
          "# React Hooks Deep Dive\n\n## Main Hooks\n\n### useState\n- Manages local component state\n- Example: const [count, setCount] = useState(0)\n\n### useEffect\n- Handles side effects\n- Runs after render\n- Can have dependencies\n\n### useContext\n- Access context without prop drilling\n- Useful for themes, authentication, etc.\n\n## Custom Hooks\n- Reuse logic between components\n- Always start with 'use'\n- Examples: useLocalStorage, useFetch\n\n## Best Practices\n1. Never call hooks inside loops or conditions\n2. Only call hooks in React components\n3. Use ESLint plugin for hooks\n4. Keep hooks simple and focused",
          "Session Notes"
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
        "Carlos Ruiz",
        now.AddDays(35),
        "Learn TypeScript best practices, advanced types, generics and how to get the most out of the type system.",
        "https://images.unsplash.com/photo-1516116216624-53e697fedbea?w=800"
    );

    if (session2.IsSuccess)
    {
      await _context.Sessions.AddAsync(session2.Value);
    }

    // Session 3: Next.js 15 Features (Scheduled) - 55 days from now
    var session3 = Session.Create(
        communityId,
        "Next.js 15: New Features",
        "Ana Martinez",
        now.AddDays(55),
        "Discover the latest Next.js 15 features, Server Components, App Router and performance optimizations.",
        "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800"
    );

    if (session3.IsSuccess)
    {
      await _context.Sessions.AddAsync(session3.Value);
    }

    // Session 4: CSS Grid & Flexbox (Canceled)
    var session4 = Session.Create(
        communityId,
        "CSS Grid & Flexbox Masterclass",
        "Roberto Silva",
        now.AddDays(30),
        "Master CSS Grid and Flexbox to create modern and responsive layouts without frameworks.",
        "https://images.unsplash.com/photo-1507721999472-8ed4421c4af2?w=800"
    );

    if (session4.IsSuccess)
    {
      session4.Value.UpdateStatus(SessionStatus.Canceled);
      await _context.Sessions.AddAsync(session4.Value);
    }
  }

  private async Task SeedPythonSessionsAsync(Guid communityId)
  {
    var now = DateTimeOffset.UtcNow;

    // Session 1: Machine Learning Basics (Completed) - 25 days ago
    var session1 = Session.CreateCompleted(
        communityId,
        "Introduction to Machine Learning with Scikit-learn",
        "Dr. Luis Fernandez",
        now.AddDays(-25),
        "Machine learning fundamentals, supervised and unsupervised algorithms with practical examples using Scikit-learn.",
        "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=800"
    );

    if (session1.IsSuccess)
    {
      session1.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=7eh4d6sabA0",
          null,
          "Machine Learning with Scikit-learn - Recording"
      );

      session1.Value.AddResource(
          SessionResourceType.Notes,
          null,
          "# Machine Learning with Scikit-learn\n\n## Types of Learning\n\n### Supervised\n- Classification (SVM, Random Forest, Logistic Regression)\n- Regression (Linear Regression, Ridge, Lasso)\n\n### Unsupervised\n- Clustering (K-Means, DBSCAN)\n- Dimensionality Reduction (PCA, t-SNE)\n\n## Workflow\n1. Load data\n2. Exploration and cleaning\n3. Feature engineering\n4. Train/test split\n5. Training\n6. Evaluation\n7. Hyperparameter tuning\n\n## Evaluation Metrics\n- Accuracy\n- Precision, Recall, F1-Score\n- ROC-AUC\n- Confusion Matrix\n\n## Additional Resources\n- Scikit-learn.org\n- Kaggle datasets\n- Google Colab for practice",
          "ML Key Concepts"
      );

      await _context.Sessions.AddAsync(session1.Value);
    }

    // Session 2: FastAPI Development (Scheduled) - 40 days from now
    var session2 = Session.Create(
        communityId,
        "Building APIs with FastAPI",
        "Patricia Torres",
        now.AddDays(40),
        "Build modern and efficient REST APIs with FastAPI, including validation, automatic documentation and async/await.",
        "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800"
    );

    if (session2.IsSuccess)
    {
      await _context.Sessions.AddAsync(session2.Value);
    }

    // Session 3: Data Analysis with Pandas (Completed) - 4 days ago
    var session3 = Session.CreateCompleted(
        communityId,
        "Data Analysis with Pandas",
        "Jorge Mendoza",
        now.AddDays(-4),
        "Advanced data analysis and manipulation techniques with Pandas, visualization and dataset cleaning.",
        "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800"
    );

    if (session3.IsSuccess)
    {
      session3.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=vmEHCJofslg",
          null,
          "Data Analysis with Pandas - Full Session"
      );

      session3.Value.AddResource(
          SessionResourceType.ExternalLink,
          "https://colab.research.google.com/",
          null,
          "Google Colab - Interactive Notebook"
      );

      await _context.Sessions.AddAsync(session3.Value);
    }

    // Session 4: Django for Beginners (Scheduled) - 60 days from now
    var session4 = Session.Create(
        communityId,
        "Django for Beginners",
        "Sandra Lopez",
        now.AddDays(60),
        "Getting started with Django, creating models, views, templates and user management.",
        "https://images.unsplash.com/photo-1580927752452-89d86da3fa0a?w=800"
    );

    if (session4.IsSuccess)
    {
      await _context.Sessions.AddAsync(session4.Value);
    }
  }

  private async Task SeedCloudSessionsAsync(Guid communityId)
  {
    var now = DateTimeOffset.UtcNow;

    // Session 1: Kubernetes Fundamentals (Completed) - 20 days ago
    var session1 = Session.CreateCompleted(
        communityId,
        "Kubernetes Fundamentals",
        "Miguel Angel Rojas",
        now.AddDays(-20),
        "Introduction to Kubernetes, pods, deployments, services and cluster architecture for container orchestration.",
        "https://images.unsplash.com/photo-1667372393119-3d4c48d07fc9?w=800"
    );

    if (session1.IsSuccess)
    {
      session1.Value.AddResource(
          SessionResourceType.Video,
          "https://www.youtube.com/watch?v=X48VuDVv0do",
          null,
          "Kubernetes Fundamentals - Recording"
      );

      session1.Value.AddResource(
          SessionResourceType.Notes,
          null,
          "# Kubernetes Fundamentals\n\n## Architecture\n\n### Control Plane\n- API Server\n- etcd (storage)\n- Scheduler\n- Controller Manager\n\n### Worker Nodes\n- kubelet\n- kube-proxy\n- Container runtime (Docker, containerd)\n\n## Main Objects\n\n### Pod\n- Basic deployment unit\n- Can contain one or more containers\n- Share network and storage\n\n### Deployment\n- Manages ReplicaSets\n- Allows rolling updates\n- Automatic rollback\n\n### Service\n- ClusterIP (internal)\n- NodePort (expose port)\n- LoadBalancer (cloud)\n\n### ConfigMap & Secret\n- Externalized configuration\n- Secrets for sensitive data\n\n## Basic Commands\n```bash\nkubectl get pods\nkubectl apply -f deployment.yaml\nkubectl logs <pod-name>\nkubectl exec -it <pod-name> -- /bin/bash\nkubectl scale deployment/app --replicas=3\n```",
          "Kubernetes Guide"
      );

      session1.Value.AddResource(
          SessionResourceType.ExternalLink,
          "https://kubernetes.io/docs/home/",
          null,
          "Kubernetes Official Documentation"
      );

      await _context.Sessions.AddAsync(session1.Value);
    }

    // Session 2: AWS Serverless Architecture (Scheduled) - 45 days from now
    var session2 = Session.Create(
        communityId,
        "Serverless Architectures on AWS",
        "Laura Jimenez",
        now.AddDays(45),
        "Design and deploy serverless applications using AWS Lambda, API Gateway, DynamoDB and other AWS services.",
        "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=800"
    );

    if (session2.IsSuccess)
    {
      await _context.Sessions.AddAsync(session2.Value);
    }

    // Session 3: Docker Deep Dive (Completed) - 10 days ago
    var session3 = Session.CreateCompleted(
        communityId,
        "Docker: From Basics to Advanced",
        "Fernando Castro",
        now.AddDays(-10),
        "Master Docker: images, containers, networking, volumes, Docker Compose and build optimization.",
        "https://images.unsplash.com/photo-1605745341075-0f92a4f2c01f?w=800"
    );

    if (session3.IsSuccess)
    {
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
        "Infrastructure as Code with Terraform",
        "Elena Vargas",
        now.AddDays(65),
        "Learn to manage infrastructure as code using Terraform, modules, state management and best practices.",
        "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800"
    );

    if (session4.IsSuccess)
    {
      await _context.Sessions.AddAsync(session4.Value);
    }

    // Session 5: CI/CD with GitHub Actions (Canceled)
    var session5 = Session.Create(
        communityId,
        "CI/CD with GitHub Actions",
        "Ricardo Pena",
        now.AddDays(14),
        "Automate your workflows with GitHub Actions, workflows, secrets and deployment strategies.",
        "https://images.unsplash.com/photo-1618401471353-b98afee0b2eb?w=800"
    );

    if (session5.IsSuccess)
    {
      session5.Value.UpdateStatus(SessionStatus.Canceled);
      await _context.Sessions.AddAsync(session5.Value);
    }
  }
}
