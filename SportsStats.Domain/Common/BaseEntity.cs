namespace SportsStats.Domain.Common
{
	public abstract class BaseEntity : IEntity
	{
		private int? _requestedHashCode;
		private int _id;

		// Основной идентификатор
		public virtual int Id
		{
			get => _id;
			protected set => _id = value;
		}
		public DateTime CreatedAt { get; protected set; } = DateTime.Now;

		// Конструктор для создания новой сущности (без Id)
		protected BaseEntity()
		{
		}

		// Конструктор с Id (для загрузки из БД)
		protected BaseEntity(int id)
		{
			_id = id;
		}

		// Сравнение сущностей по Id и типу
		public override bool Equals(object? obj)
		{
			if (obj == null || !(obj is BaseEntity other))
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (GetType() != other.GetType())
				return false;

			if (Id == 0 || other.Id == 0)
				return false;

			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			if (!_requestedHashCode.HasValue)
				_requestedHashCode = (GetType().GetHashCode() * 907) + Id.GetHashCode();

			return _requestedHashCode.Value;
		}

		// Для сравнения (== и !=)
		public static bool operator ==(BaseEntity? left, BaseEntity? right)
		{
			if (Equals(left, null))
				return Equals(right, null);

			return left.Equals(right);
		}

		public static bool operator !=(BaseEntity? left, BaseEntity? right)
		{
			return !(left == right);
		}

		// Проверка, что сущность сохранена (Id > 0)
		public bool IsTransient()
		{
			return Id == 0;
		}
	}
}